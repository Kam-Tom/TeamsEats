import React from 'react';
import {
    Body1,
    Subtitle2,
    Subtitle1,
    Card,
    Dropdown,
    Option,
    Button
} from "@fluentui/react-components";
import { makeStyles, tokens } from '@fluentui/react-components';
import ItemCard from './ItemCard';
import { DetailsData } from "../../models/DetailsData";
import { Status } from "../../models/Status";
import ItemForm from './ItemForm';
import { TeamsFxContext } from '../Context';
import Timer from '../Shared/Timer';

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
    }
});



const DetailsCard: React.FC<DetailsData> = ({ id, isOwnedByUser, authorName, restaurant, phoneNumber, bankAccount, minimalPrice, deliveryCost, minimalPriceForFreeDelivery, orderItems, status, closingTime }) => {

    const { teamsUserCredential } = React.useContext(TeamsFxContext);
    const classes = useStyles();

    const handleChangeStatus = async (newStatus: Status) => {
        if (!teamsUserCredential) return;

        const token = await teamsUserCredential.getToken("");
        const changeStatusDTO = {
            GroupOrderID: id,
            Status: newStatus
        };

        try {
            const response = await fetch('https://localhost:7125/GroupOrderDetails', {
                method: 'PATCH',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token!.token}`
                },
                body: JSON.stringify(changeStatusDTO)
            });

            if (!response.ok) {
                console.error(`Failed to change status: ${response.statusText}`);
            }
        } catch (error) {
            console.error('Error during status change:', error);
        }
    };

    const handleDeleteOrder = async () => {
        if (!teamsUserCredential) return;

        const token = await teamsUserCredential.getToken("");

        try {
            const response = await fetch(`https://localhost:7125/GroupOrderDetails/${id}`, {
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
                        <Body1>{`Minimal price: ${minimalPrice}`}</Body1>
                        <Body1>Closing Time: <Timer targetTime={closingTime} /></Body1>
                        <Body1>{`Author: ${authorName}`}</Body1>
                    </div>
                </div>
                <div className={classes.section}>
                    <Subtitle2>Delivery</Subtitle2>
                    <div className={classes.grid}>
                        <Body1>{`Minimal for free delivery: ${minimalPriceForFreeDelivery}`}</Body1>
                        <Body1>{`Status: ${Status[status]}`}</Body1>
                        <Body1>{`Delivery fee: ${deliveryCost.toFixed(2)}`}</Body1>
                    </div>
                </div>
                <div className={classes.section}>
                    <Subtitle2>Payment</Subtitle2>
                    <div className={classes.grid}>
                        <Body1>{`Phone number: ${phoneNumber}`}</Body1>
                        <Body1>{`Bank account: ${bankAccount}`}</Body1>
                    </div>
                </div>
                {isOwnedByUser && (
                    <div>
                        <div className={classes.dropdownContainer}>
                            <label htmlFor="status-dropdown" className={classes.label}>Change Status</label>
                            <Dropdown
                                id="status-dropdown"
                                placeholder="Select status"
                                onActiveOptionChange={onActiveOptionChange}
                                defaultValue={Status[status]}
                            >
                                <Option value={Status.Open}>Open</Option>
                                <Option value={Status.Closed}>Closed</Option>
                                <Option value={Status.Delivered}>Delivered</Option>
                            </Dropdown>
                        </div>
                        <Button className={classes.deleteButton} onClick={handleDeleteOrder}>Delete Order</Button>
                    </div>
                )}
                <div className={classes.section}>
                    <Subtitle2>Items</Subtitle2>
                    <ItemForm groupOrderId={id} />
                    {orderItems.map((item) => (
                        <ItemCard key={item.id} {...item} groupOrderId={id} canComment={isOwnedByUser} deliveryCost={deliveryCost} />
                    ))}
                </div>
            </Card>
        </div>
    );
}

export default DetailsCard;
