import React, { useContext, useEffect, useState, useRef } from 'react';
import { useNavigate } from 'react-router-dom';
import { TeamsFxContext } from '../Context';
import { DetailsData } from '../../models/DetailsData';
import DetailsCard from './DetailsCard';
import { makeStyles, Button, tokens, Spinner, Body1 } from '@fluentui/react-components';
import { Icon } from '@fluentui/react/lib/Icon';
import * as signalR from '@microsoft/signalr';

const useStyles = makeStyles({
    detailsPage: {
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'center',
        width: '98vw',
        minHeight: '100vh',
        position: 'relative',
        overflowY: 'hidden',
    },
    backButton: {
        position: 'absolute',
        top: '10px',
        right: '10px',
        aspectRatio: '1/1',
        textAlign: 'center',
        display: 'flex',
        width: '2rem',
        justifyContent: 'center',
        alignItems: 'center',
        borderRadius: tokens.borderRadiusCircular,
        backgroundColor: tokens.colorNeutralBackgroundInverted,
        zIndex: "2"
    },
    buttonIcon: {
        fontSize: '1rem',
        color: tokens.colorNeutralForegroundInverted,
    },
    errorWrapper: {
        display: 'flex',
        flexDirection: 'column',
        alignItems: 'center',
        justifyContent: 'center',
        textAlign: 'center',
    },
    errorButton: {
        marginTop: '1rem',
    }
});

interface DetailsPageProps {
    id: string;
}

const DetailsPage: React.FC<DetailsPageProps> = ({ id }) => {
    const { teamsUserCredential } = useContext(TeamsFxContext);
    const [detailsData, setDetailsData] = useState<DetailsData | null>(null);
    const [loading, setLoading] = useState<boolean>(true);
    const [error, setError] = useState<boolean>(false);
    const navigate = useNavigate();
    const classes = useStyles();
    const connectionRef = useRef<signalR.HubConnection | null>(null);

    const fetchDetail = async (id: string) => {
        try {
            if (!teamsUserCredential) return;
            const token = await teamsUserCredential.getToken("");

            const response = await fetch(`https://localhost:7125/order/${id}/detail`, {
                method: 'GET',
                headers: {
                    'Authorization': `Bearer ${token!.token}`
                }
            });

            if (response.ok) {
                const groupOrder: DetailsData = await response.json();
                return groupOrder;
            } else {
                throw new Error(`Failed to fetch data: ${response.statusText}`);
            }
        } catch (error) {
            console.error('Error during login or fetching data:', error);
            setError(true);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        const loadData = async () => {
            if (id) {
                const data = await fetchDetail(id);
                if (data) {
                    setDetailsData(data);
                }
            }
        };

        loadData();
    }, [id]);

    useEffect(() => {
        const connectToSignalR = async () => {
            if (!teamsUserCredential || connectionRef.current) return;
            const token = await teamsUserCredential.getToken("");

            const connection = new signalR.HubConnectionBuilder()
                .withUrl('https://localhost:7125/orderHub', {
                    accessTokenFactory: () => token!.token
                })
                .withAutomaticReconnect()
                .build();

            connection.on('OrderUpdated', async (orderId: number) => {
                if (id === orderId.toString()) {
                    const updatedDetails = await fetchDetail(orderId.toString());
                    if (updatedDetails) {
                        setDetailsData(updatedDetails);
                    }
                }
            });
            connection.on('OrderDeleted', async (groupOrderId: number) => {
                if (id === groupOrderId.toString()) {
                    handleBackToList();
                }
            });
            try {
                await connection.start();
                connectionRef.current = connection;
            } catch (err) {
                console.error('Error connecting to SignalR:', err);
            }
        };

        connectToSignalR();

        return () => {
            if (connectionRef.current) {
                connectionRef.current.stop();
                connectionRef.current = null;
            }
        };
    }, [teamsUserCredential, id]);

    const handleBackToList = () => {
        navigate('/');
    };

    if (loading) {
        return (
            <div className={classes.detailsPage}>
                <Spinner label="Loading..." />
            </div>
        );
    }

    if (error || !detailsData) {
        return (
            <div className={classes.detailsPage}>
                <button onClick={handleBackToList} className={classes.backButton}>
                    <Icon iconName="Back" className={classes.buttonIcon} />
                </button>
                <div className={classes.errorWrapper}>
                    <Body1>Failed to load the order details or the order was deleted.</Body1>
                    <Button className={classes.errorButton} onClick={handleBackToList}>Back</Button>
                </div>
            </div>
        );
    }

    return (
        <div className={classes.detailsPage}>
            <button onClick={handleBackToList} className={classes.backButton}>
                <Icon iconName="Back" className={classes.buttonIcon} />
            </button>
            <DetailsCard {...detailsData} />
        </div>
    );
};

export default DetailsPage;
