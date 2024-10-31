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
        width: '100%',
        position: 'relative',
        marginBottom: '1rem',
    },
    inactive: {
        opacity: 0.5,
    },
    delivered: {
        backgroundColor: tokens.colorPaletteMarigoldBorder2,
    },
    ordered: {
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
    description: {
        textWrap: 'nowrap',
        textOverflow: 'ellipsis',
        overflow: 'hidden',
        whiteSpace: 'nowrap', 
        maxWidth: '120px', 
    },
    header: {
        width: '100%',
    },
    cardBody: {
        display: 'flex',
        padding: '0.5rem',
        justifyContent: 'space-around', 
        flex: '1',
        flexWrap: 'wrap', 
    },
    cardBodySection: {
        display: 'flex',
        flexDirection: 'column',
        alignItems: 'center',
        justifyContent: 'flex-start',
        minWidth: '100px', 
        flexGrow: 1, 
        textAlign: 'center', 
    },
    sectionHeader: {
        color: tokens.colorNeutralStrokeAccessible,
        fontSize: tokens.fontSizeBase200,
    },
    sectionMain: {
        color: tokens.colorBrandForegroundOnLight,
        fontSize: tokens.fontSizeBase400,
        fontWeight: tokens.fontWeightSemibold,
    },
    minimalPrice: {
        color: tokens.colorNeutralStrokeAccessible,
        fontSize: tokens.fontSizeBase200,
    },
    currentDeliveryFeeGreen: {
        color: 'green',
    },
    currentPriceRed: {
        color: 'red',
    },
    none: {
        display: 'none',
    }
});

interface OrderCardProps extends OrderData {
    onClick: (id: number) => void;
}

const OrderCard: React.FC<OrderCardProps> = ({
    authorName,
    authorPhoto,
    currentDeliveryFee,
    myCost,
    currentPrice,
    minimalPrice,
    restaurant,
    status,
    id,
    isOwner,
    closingTime,
    onClick
}) => {
    const classes = useStyles();
    const canClick = status === Status.Open || isOwner || myCost!==0;

    const cardStyle = mergeClasses(classes.card, (!canClick) && classes.inactive);
    const iconRoundedStyle = mergeClasses(classes.iconRounded, myCost !== 0 && classes.hasItem, isOwner && classes.owner);
    const iconCircleStyle = mergeClasses(classes.iconCircle, (status === Status.Open) && classes.open,
        (status === Status.Ordered) && classes.ordered,
        (status === Status.Delivered) && classes.delivered);

    const photoSrc = authorPhoto ? (authorPhoto.startsWith('data:image') ? authorPhoto : `data:image/jpeg;base64,${authorPhoto}`) : '/path/to/default/avatar.png';

    return (
        <Card
            orientation="horizontal"
            className={cardStyle}
            onClick={() => { if (canClick) onClick(id) }}>
            <CardHeader
                image={<Avatar name={authorName} image={{ src: photoSrc }} />}
                header={<Text weight="semibold" className={classes.header}> {restaurant}</Text>}
                description={<Caption1 className={classes.description}> {authorName} </Caption1>}
            />
            <div className={iconCircleStyle}></div>
            <div className={iconRoundedStyle}></div>

                <div className={classes.cardBody}>

                        <div className={classes.cardBodySection}>
                            <span className={classes.sectionHeader}> Delivery Fee </span>
                            <span className={currentDeliveryFee === 0 ? classes.currentDeliveryFeeGreen : classes.sectionMain}>
                                {currentDeliveryFee}$
                            </span>
                        </div>

                        <div className={classes.cardBodySection}>
                            <span className={classes.sectionHeader}> Order Sum </span>
                            <span className={currentPrice < minimalPrice ? classes.currentPriceRed : classes.sectionMain}>
                                {currentPrice}$ / <span className={classes.minimalPrice}>{minimalPrice}$</span>
                            </span>
                        </div>

                <div className={myCost !== 0 ? classes.cardBodySection : classes.none}>
                    <span className={classes.sectionHeader}> My Cost </span>
                    <span className={classes.sectionMain}> {myCost + currentDeliveryFee}$ </span>
                        </div>

                        <div className={classes.cardBodySection}>
                            <span className={classes.sectionHeader}> Closing Time </span>
                            <Timer targetTime={closingTime}></Timer>
                        </div>

                </div>

        </Card>
    );
};

export default OrderCard;