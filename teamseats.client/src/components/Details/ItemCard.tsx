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
    Input,
    tokens,
} from '@fluentui/react-components';
import { Icon } from '@fluentui/react/lib/Icon';
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
        display: 'flex',

    },
    deleteButton: {
        backgroundColor: tokens.colorTransparentBackground,
        position: 'absolute',
        bottom: '0px',
        left: '0px',
        width: '0.2rem',
        height: '0.5rem',
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'center',
    },
    buttonIcon: {
        fontSize: '1rem',
        color: tokens.colorPaletteRedBackground3,
    },
});

interface ItemCardProps extends ItemData {
    canComment: boolean;
    canEdit: boolean;
}

const ItemCard: React.FC<ItemCardProps> = ({
    authorName,
    authorPhoto,
    isOwner,
    canComment,
    orderId,
    dish,
    price,
    additionalInfo,
    id,
    canEdit,
}) => {
    const [comment, setComment] = useState('');
    const popoverId = useId();
    const { teamsUserCredential } = useContext(TeamsFxContext);
    const classes = useStyles();

    const photoSrc = authorPhoto ? (authorPhoto.startsWith('data:image') ? authorPhoto : `data:image/jpeg;base64,${authorPhoto}`) : '/path/to/default/avatar.png';

    const handleCommentChange = (event: ChangeEvent<HTMLInputElement>) => {
        setComment(event.target.value);
    };

    const handleComment = async () => {
        if (!teamsUserCredential) return;

        const token = await teamsUserCredential.getToken("");

        try {
            const response = await fetch(`https://localhost:7125/item/${id}/comments`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token!.token}`
                },
                body: JSON.stringify(comment)
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

    const handleDelete = async () => {
        if (!teamsUserCredential) return;

        const token = await teamsUserCredential.getToken("");

        try {
            const response = await fetch(`https://localhost:7125/item/${id}`, {
                method: 'DELETE',
                headers: {
                    'Authorization': `Bearer ${token!.token}`
                }
            });

            if (!response.ok) {
                console.error(`Failed to delete order item: ${response.statusText}`);
            }
        } catch (error) {
            console.error('Error during order item deletion:', error);
        }
    };

    return (
        <Card orientation="horizontal" className={classes.card}>
            <CardHeader
                image={<Avatar name={authorName} image={{ src: photoSrc }} />}
                header={<Tooltip content={dish} relationship="description">
                    <Text className={classes.dishName}>{dish}</Text>
                </Tooltip>}
                description={<Caption1>{authorName}</Caption1>}
            />

            <div className={classes.content}>
                <Text>{`Price: ${price }$ `}</Text>
                <Tooltip content={additionalInfo} relationship="description">
                    <Text className={classes.additionalInfo}>{additionalInfo}</Text>
                </Tooltip>
            </div>
            {canEdit && (
                <button onClick={handleDelete} className={classes.deleteButton}>
                    <Icon iconName="Delete" className={classes.buttonIcon} />
                </button>
            )}
            <div className={classes.buttonWrapper}>
                {canEdit && (
                    <EditForm dish={dish} itemId={id} orderId={orderId} price={price.toString()} additionalInfo={additionalInfo} />
                )}
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
                                    <Button onClick={handleComment}>Send</Button>
                                </div>
                            </div>
                        </PopoverSurface>
                    </Popover>
                )}
            </div>
        </Card>
    );
}

export default ItemCard;
