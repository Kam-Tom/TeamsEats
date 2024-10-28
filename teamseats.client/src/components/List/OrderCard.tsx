import React from 'react';
import {
    Text,
    Caption1,
    Avatar,
    tokens
} from '@fluentui/react-components';
import { Card, CardHeader, makeStyles, mergeClasses } from '@fluentui/react-components';
import { OrderData } from '../../models/OrderData';
import { Status } from '../../models/Status';
import Timer from '../Shared/Timer';

const useStyles = makeStyles({
    card: {
        width: '300px',
        position: 'relative',
    },
    inactive: {
        opacity: 0.5,
    },
    delivered: {
        backgroundColor: tokens.colorPaletteMarigoldBorder2,
    },
    closed: {
        backgroundColor: tokens.colorPaletteRedBackground3,
    },
    open: {
        backgroundColor: tokens.colorPaletteLightGreenBackground3,
    },
    owner: {
        backgroundColor: tokens.colorBrandStroke1,
    },
    hasItem: {
        backgroundColor: tokens.colorPaletteYellowBackground2,
    },
    iconCircle: {
        width: '16px',
        height: '16px',
        position: 'absolute',
        top: '8px',
        right: '8px',
        borderRadius: tokens.borderRadiusCircular,
    },
    iconRounded: {
        width: '16px',
        height: '8px',
        position: 'absolute',
        top: '32px',
        right: '8px',
        borderRadius: tokens.borderRadiusXLarge,
    },
    bottomRow: {
        display: 'flex',
        justifyContent: 'space-around',

        width:'100%',
    },
    topRow: {
        color: 'red',
    }
});

interface OrderCardProps extends OrderData {
    onClick: (id: number) => void;
}

const GroupOrderCard: React.FC<OrderCardProps> = ({ authorName, authorPhoto, deliveryCost, restaurant, status, id, isOwner, isParticipating, closingTime, onClick }) => {
    const classes = useStyles();
    const canClick = status === Status.Open || isOwner || isParticipating;
    const cardStyle = mergeClasses(classes.card, (!canClick) && classes.inactive);
    const iconRoundedStyle = mergeClasses(classes.iconRounded, isParticipating && classes.hasItem, isOwner && classes.owner);
    const iconCircleStyle = mergeClasses(classes.iconCircle, (status === Status.Open) && classes.open,
        (status === Status.Closed) && classes.closed,
        (status === Status.Delivered) && classes.delivered);

    const photoSrc = authorPhoto ? (authorPhoto.startsWith('data:image') ? authorPhoto : `data:image/jpeg;base64,${authorPhoto}`) : '/path/to/default/avatar.png';


    return (
        <Card
            orientation= "horizontal"
    className = { cardStyle }
    onClick = {() => { if (canClick) onClick(id) }}>
    <CardHeader
                image={ <Avatar name={ authorName } image = {{ src: photoSrc } } />}
                header={< Text weight="semibold" className={classes.topRow}> {restaurant}</Text>}
                description={ < Caption1> {authorName} </Caption1>

                }
            />
    < div className = { iconCircleStyle } > </div>
        < div className = { iconRoundedStyle } > </div>
            <div className={classes.bottomRow}>
                < Text> {deliveryCost.toFixed(2)}$ </Text>
                <Timer targetTime={closingTime}></Timer>

            </div>
            </Card>
    );
}

export default GroupOrderCard;
