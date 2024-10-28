import { useNavigate } from "react-router-dom";
import React, { useEffect, useState, useContext, useRef } from 'react';
import { makeStyles, Button, Spinner, Body1, tokens } from '@fluentui/react-components';
import { TeamsFxContext } from '../Context';
import * as signalR from '@microsoft/signalr';
import { OrderData } from '../../models/OrderData';
import OrderCard from './OrderCard';
import OrderForm from './OrderForm';

const useStyles = makeStyles({
    container: {
        display: 'flex',
        flexDirection: 'row',
        width: '100vw',
        height: '100vh',
        justifyContent: 'center',
        alignItems: 'center',
    },
    imageContainer: {
        flex: '1',
        display: 'flex',
        justifyContent: 'center',
        padding: '5%',
        alignItems: 'center',
        '@media (max-width: 768px)': {
            display: 'none',
        },
    },
    image: {
        maxHeight: '400px',
        aspectRatio: '1/1',
    },
    listContainer: {
        flex: '2',
        '@media (max-width: 768px)': {
            flex: '1',
        },
        display: 'flex',
        flexDirection: 'column',
        justifyContent: 'center',
        alignItems: 'center',
    },
    cards: {
        backgroundColor: tokens.colorNeutralBackground2,
        height: '80vh',
        overflowY: 'auto',
        minWidth: '300px',

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
    },
    formContainer: {
        position: 'fixed',
        bottom: 0,
        width: '100%',
        backgroundColor: '#fff',
        padding: '1rem',
        boxShadow: '0 -2px 4px rgba(0,0,0,0.1)',
    }
});

const ListPage: React.FC = () => {
    const navigate = useNavigate();
    const { teamsUserCredential } = useContext(TeamsFxContext);
    const [cardsData, setCardsData] = useState<OrderData[]>([]);
    const [loading, setLoading] = useState<boolean>(true);
    const [error, setError] = useState<boolean>(false);
    const connectionRef = useRef<signalR.HubConnection | null>(null);
    const classes = useStyles();

    const fetchData = async () => {
        try {
            if (!teamsUserCredential) return;
            const token = await teamsUserCredential.getToken("");

            const response = await fetch('https://localhost:7125/order', {
                method: 'GET',
                headers: {
                    'Authorization': `Bearer ${token!.token}`
                }
            });

            if (response.ok) {
                const data: OrderData[] = await response.json();
                return data;
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

    const fetchOne = async (id: number) => {
        try {
            if (!teamsUserCredential) return;
            const token = await teamsUserCredential.getToken("");

            const response = await fetch(`https://localhost:7125/order/${id}`, {
                method: 'GET',
                headers: {
                    'Authorization': `Bearer ${token!.token}`
                }
            });

            if (response.ok) {
                const groupOrder: OrderData = await response.json();
                return groupOrder;
            } else {
                throw new Error(`Failed to fetch data: ${response.statusText}`);
            }
        } catch (error) {
            console.error('Error during login or fetching data:', error);
            setError(true);
        }
    };

    useEffect(() => {
        const loadData = async () => {
            const data = await fetchData();
            if (data) {
                setCardsData(data);
            }
        }

        loadData();
    }, [teamsUserCredential]);

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

            connection.on('OrderCreated', async (id: number) => {
                const data = await fetchOne(id);
                if (data) {
                    setCardsData(prevData => [...prevData, data]);
                }
            });

            connection.on('OrderDeleted', async (id: number) => {
                setCardsData(prevData => prevData.filter(order => order.id !== id));
            });

            connection.on('OrderUpdated', async (id: number) => {
                const data = await fetchOne(id);
                if (data) {
                    setCardsData(prevData => prevData.map(order => order.id === data.id ? data : order));
                }
            });

            try {
                await connection.start();
                connectionRef.current = connection;
            } catch (err) {
                console.error('Error connecting to SignalR:', err);
                setError(true);
            }
        };

        connectToSignalR();

        return () => {
            if (connectionRef.current) {
                connectionRef.current.stop();
                connectionRef.current = null;
            }
        };
    }, [teamsUserCredential]);

    const handleCardClick = (id: number) => {
        navigate(`/tab/${id}`);
    };

    if (loading) {
        return (
            <div className={classes.container}>
                <Spinner label="Loading..." />
            </div>
        );
    }

    if (error) {
        return (
            <div className={classes.container}>
                <div className={classes.errorWrapper}>
                    <Body1>Failed to load the group orders.</Body1>
                    <Button className={classes.errorButton} onClick={() => window.location.reload()}>Retry</Button>
                </div>
            </div>
        );
    }

    return (
        <div className={classes.container}>
            <div className={classes.imageContainer}>
                <img src="/placeholder_logo.png" alt="Placeholder Logo" className={classes.image} />
            </div>
            <div className={classes.listContainer}>
                <div className={classes.cards}>
                    {cardsData.length === 0 ? (
                        <Body1>No orders</Body1>
                    ) : (
                        cardsData.map((cardData) => (
                            <OrderCard
                                key={cardData.id}
                                {...cardData}
                                onClick={() => handleCardClick(cardData.id)}
                            />
                        ))
                    )}
                </div>
                <OrderForm />
            </div>

        </div>
    );
};

export default ListPage;
