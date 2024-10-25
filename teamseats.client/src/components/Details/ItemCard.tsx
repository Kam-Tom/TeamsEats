import React, { useState, ChangeEvent, useContext } from 'react';
import {
    Text,
    Caption1,
    Avatar,
    Tooltip,
    Button,
    Popover,
    PopoverTrigger,
    PopoverSurface,
    useId,
    Input
} from '@fluentui/react-components';
import { Card, CardHeader, makeStyles } from '@fluentui/react-components';
import { ItemData } from '../../models/ItemData';
import { TeamsFxContext } from '../Context';
import EditForm from './EditForm';

const useStyles = makeStyles({
    card: {
        position: 'relative',
    },
    content: {
        display: 'flex',
        flexDirection: 'column',
        padding: '0px 10px',
    },
    additionalInfo: {
        whiteSpace: 'nowrap',
        overflow: 'hidden',
        textOverflow: 'ellipsis',
        maxWidth: '200px',
    },
    dishName: {
        whiteSpace: 'nowrap',
        overflow: 'hidden',
        textOverflow: 'ellipsis',
        maxWidth: '100px',
        fontWeight: 'bold'
    },
    popoverContent: {
        padding: '10px',
    },
    contentHeader: {
        marginBottom: '10px',
    },
    popoverActions: {
        marginTop: '10px',
        display: 'flex',
        justifyContent: 'flex-end',
    },
    buttonWrapper: {
        position: 'absolute',
        right: '1rem',
        top: '50%',
        transform: 'translateY(-50%)',
    }

});

interface OrderItemCardProps extends ItemData {
    canComment: boolean;
    deliveryCost: number;
}

const OrderItemCard: React.FC<OrderItemCardProps> = ({
    authorName,
    authorPhoto,
    isOwner,
    canComment,
    groupOrderId,
    dishName,
    price,
    additionalInfo,
    id,
    deliveryCost,

}) => {
    const [comment, setComment] = useState('');
    const popoverId = useId();
    const { teamsUserCredential } = useContext(TeamsFxContext);
    const classes = useStyles();

    const photoSrc = authorPhoto ? (authorPhoto.startsWith('data:image') ? authorPhoto : `data:image/jpeg;base64,${authorPhoto}`) : '/path/to/default/avatar.png';


    const handleCommentChange = (event: ChangeEvent<HTMLInputElement>) => {
        setComment(event.target.value);
    };

    const handleSendComment = async () => {
        if (!teamsUserCredential) return;

        const token = await teamsUserCredential.getToken("");
        const commentOrderItemDTO = {
            OrderItemID: id,
            Message: comment
        };

        try {
            const response = await fetch('https://localhost:7125/OrderItem/SendComment', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token!.token}`
                },
                body: JSON.stringify(commentOrderItemDTO)
            });

            if (response.ok) {
                setComment('');
            } else {
                console.error(`Failed to submit comment: ${response.statusText}`);
            }
        } catch (error) {
            console.error('Error during comment submission:', error);
        }
    };

    return (
        <Card orientation="horizontal" className={classes.card}>
            <CardHeader
                image={<Avatar name={authorName} image={{ src: photoSrc }} />}
                header={<Tooltip content={dishName} relationship="description">
                    <Text className={classes.dishName}>{dishName}</Text>
                </Tooltip>}
                description={<Caption1>{authorName}</Caption1>}
            />

            <div className={classes.content}>
                <Tooltip content={`${price.toFixed(2)}$ + ${deliveryCost.toFixed(2)}$`} relationship="description">
                    <Text>{`Price: ${(price + deliveryCost).toFixed(2)}$ `}</Text>
                </Tooltip>
                <Tooltip content={additionalInfo} relationship="description">
                    <Text className={classes.additionalInfo}>{additionalInfo}</Text>
                </Tooltip>
            </div>
            <div className={classes.buttonWrapper}>
                {isOwner && <EditForm dishName={dishName} itemId={id} groupOrderId={groupOrderId} price={price.toString()} additionalInfo={additionalInfo}  ></EditForm>}
                {(canComment && !isOwner) && (
                    <Popover trapFocus>
                        <PopoverTrigger disableButtonEnhancement>
                            <Button>Comment</Button>
                        </PopoverTrigger>
                        <PopoverSurface aria-labelledby={popoverId}>
                            <div className={classes.popoverContent}>
                                <h3 id={popoverId} className={classes.contentHeader}>Add Comment</h3>
                                <Input
                                    value={comment}
                                    onChange={handleCommentChange}
                                    placeholder="Write your comment here"
                                />
                                <div className={classes.popoverActions}>
                                    <Button onClick={handleSendComment}>Send</Button>
                                </div>
                            </div>
                        </PopoverSurface>
                    </Popover>
                )}
            </div>
        </Card>
    );
}

export default OrderItemCard;
