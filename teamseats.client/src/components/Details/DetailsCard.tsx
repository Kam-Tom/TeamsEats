import React, { useState } from 'react';
import {
    Body1,
    Subtitle2,
    Subtitle1,
    Card,
    Dropdown,
    Option,
    Button,
    Tooltip
} from "@fluentui/react-components";
import { makeStyles, tokens } from '@fluentui/react-components';
import ItemCard from './ItemCard';
import { DetailsData } from "../../models/DetailsData";
import { Status } from "../../models/Status";
import ItemForm from './ItemForm';
import { TeamsFxContext } from '../Context';
import Timer from '../Shared/Timer';
import { ItemData } from '../../models/ItemData';

const useStyles = makeStyles({
    wrapper: {
        padding: '16px',
        width: '600px',
    },
    section: {
        marginBottom: '16px',
    },
    grid: {
        display: 'grid',
        gridTemplateColumns: '1fr 1fr',
        gap: '8px',
    },
    dropdownContainer: {
        marginTop: '16px',
    },
    label: {
        marginBottom: '8px',
        display: 'block',
    },
    deleteButton: {
        marginTop: '16px',
        backgroundColor: tokens.colorPaletteRedBackground3,
        color: tokens.colorNeutralForegroundInverted,
    },
    toPay: {
        whiteSpace: 'nowrap',
        overflow: 'hidden',
        textOverflow: 'ellipsis',
        maxWidth: '200px',
    },
    errorMessage: {
        color: tokens.colorPaletteRedForeground2,
        backgroundColor: tokens.colorPaletteRedBackground2,
        border: `1px solid ${tokens.colorPaletteRedBorder2}`,
        padding: '12px 16px',
        borderRadius: '6px',
        marginTop: '16px',
        fontSize: '14px',
        lineHeight: '1.5',
        display: 'flex',
        alignItems: 'center',
        gap: '8px',
    },
});

const DetailsCard: React.FC<DetailsData> = ({ id, isOwner, authorName, restaurant, phoneNumber, bankAccount, currentPrice, minimalPrice, currentDeliveryFee, minimalPriceForFreeDelivery, items, status, closingTime, myCost }) => {

    const { teamsUserCredential } = React.useContext(TeamsFxContext);
    const classes = useStyles();
    const [error, setError] = useState<string | null>(null);


    const handleChangeStatus = async (newStatus: Status) => {
        if (!teamsUserCredential) return;

        const token = await teamsUserCredential.getToken("");

        try {
            const response = await fetch(`https://localhost:7125/order/${id}`, {
                method: 'PATCH',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token!.token}`
                },
                body: JSON.stringify(newStatus)
            });

            if (!response.ok) {
                const errorMessage = await response.json();
                setError(errorMessage.detail);
            } else {
                setError(null);
            }
        } catch (error) {
            console.error('Error during status change:', error);
            setError("A network error occurred. Please try again.");
        }
    };
    const handleDeleteOrder = async () => {
        if (!teamsUserCredential) return;

        const token = await teamsUserCredential.getToken("");

        try {
            const response = await fetch(`https://localhost:7125/order/${id}`, {
                method: 'DELETE',
                headers: {
                    'Authorization': `Bearer ${token!.token}`
                }
            });

            if (!response.ok) {
                console.error(`Failed to delete order: ${response.statusText}`);
            }
        } catch (error) {
            console.error('Error during order deletion:', error);
        }
    };

    const onActiveOptionChange = (_, data) => {
        const newStatus = data.nextOption.value as Status;
        if (newStatus !== status) {
            handleChangeStatus(newStatus);
        }
    };

    return (
        <div className={classes.wrapper}>
            <Card>
                <div className={classes.section}>
                    <Subtitle1>Details</Subtitle1>
                    <div className={classes.grid}>
                        <Body1>{`Restaurant: ${restaurant}`}</Body1>
                        <Body1>{`Minimal price: ${minimalPrice}$`}</Body1>
                        <Body1>Closing Time: <Timer targetTime={closingTime} /></Body1>
                        <Body1>{`Author: ${authorName}`}</Body1>
                        <Body1>{`Current Price: ${currentPrice}$`}</Body1>
                        <Body1>{`Minimal Price: ${minimalPrice}$`}</Body1>
                    </div>
                </div>
                <div className={classes.section}>
                    <Subtitle2>Delivery</Subtitle2>
                    <div className={classes.grid}>
                        <Body1>{`Delivery Fee: ${currentDeliveryFee}$`}</Body1>
                        <Body1>{`Minimal for free delivery: ${minimalPriceForFreeDelivery}$`}</Body1>
                        <Body1>{`Status: ${Status[status]}`}</Body1>
                    </div>
                </div>
                <div className={classes.section}>
                    <Subtitle2>Payment</Subtitle2>
                    <div className={classes.grid}>
                        <Body1>{`Phone number: ${phoneNumber}`}</Body1>
                        <Body1>{`Bank account: ${bankAccount}`}</Body1>
                        {myCost > 0 && (
                            <Tooltip content={`${myCost}$ + ${currentDeliveryFee}$`} relationship="description">
                                <Body1 className={classes.toPay}>{`To Pay: ${myCost + currentDeliveryFee}$`}</Body1>
                            </Tooltip>
                        )}
                    </div>
                </div>
                {error && <div className={classes.errorMessage}>{error}</div>}
                {isOwner && (
                    <div>
                        <div className={classes.dropdownContainer}>
                            <label htmlFor="status-dropdown" className={classes.label}>Change Status</label>
                            <Dropdown
                                id="status-dropdown"
                                placeholder="Select status"
                                onActiveOptionChange={onActiveOptionChange}
                                value={Status[status]}
                            >
                                <Option value={Status.Open}>Open</Option>
                                <Option value={Status.Ordered}>Ordered</Option>
                                <Option value={Status.Delivered}>Delivered</Option>
                            </Dropdown>
                        </div>
                        <Button className={classes.deleteButton} onClick={handleDeleteOrder}>Delete Order</Button>
                    </div>
                )}
                <div className={classes.section}>
                    <Subtitle2>Items</Subtitle2>
                    <ItemForm orderId={id} />
                    {items.map((item: ItemData) => (
                        <ItemCard key={item.id} {...item} orderId={id} canComment={isOwner} canEdit={item.isOwner && status == Status.Open} />
                    ))}
                </div>
            </Card>
        </div>
    );
}

export default DetailsCard;
