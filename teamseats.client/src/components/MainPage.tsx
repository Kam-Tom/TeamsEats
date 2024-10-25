import React, { useContext, useRef, useState, useEffect } from 'react';
import GroupOrderList from './GroupOrder/GroupOrderList';
import GroupOrderForm from './GroupOrder/GroupOrderForm';
import './MainPage.module.css';
import GroupOrderDetailsCard from './GroupOrder/GroupOrderDetailsCard';
import styles from './MainPage.module.css';
import { GroupOrderDetailsCardData } from '../models/DetailsData';
import { TeamsFxContext } from './Context';
import * as signalR from '@microsoft/signalr';

const MainPage: React.FC = () => {
    const { teamsUserCredential } = useContext(TeamsFxContext);
    const connectionRef = useRef<signalR.HubConnection | null>(null);
    const [detailsData, setDetailsData] = useState<GroupOrderDetailsCardData | null>(null);

    const fetchOne = async (id: number) => {
        try {
            if (!teamsUserCredential) return;
            const token = await teamsUserCredential.getToken("");

            const response = await fetch(`https://localhost:7125/GroupOrderDetails/${id}`, {
                method: 'GET',
                headers: {
                    'Authorization': `Bearer ${token!.token}`
                }
            });

            if (response.ok) {
                const groupOrder: GroupOrderDetailsCardData = await response.json();
                return groupOrder;
            } else {
                console.error(`Failed to fetch data: ${response.statusText}`);
            }
        } catch (error) {
            console.error('Error during login or fetching data:', error);
        }
    };

    const loadData = async (id: number) => {
        const data = await fetchOne(id);
        if (data) {        
            setDetailsData(data);
        }
    }

    useEffect(() => {
        const connectToSignalR = async () => {
            if (!teamsUserCredential || connectionRef.current) return;
            const token = await teamsUserCredential.getToken("");

            const connection = new signalR.HubConnectionBuilder()
                .withUrl('https://localhost:7125/groupOrderHub', {
                    accessTokenFactory: () => token!.token
                })
                .withAutomaticReconnect()
                .build();

            connection.on('GroupOrderDeleted', async (groupOrderId: number) => {
                if (detailsData && detailsData.id == groupOrderId) {
                    setDetailsData(null);
                }
            });

            connection.on('GroupOrderUpdated', async (groupOrderId: number) => {
                if (detailsData && detailsData.id == groupOrderId) {
                    
                    const data = await fetchOne(groupOrderId);
                    if (data) {
                        setDetailsData(data);
                    }
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
    }, [teamsUserCredential]);

    return (
        <div className={styles['main-page']}>
            <div className={styles['details']}>
                {detailsData && <GroupOrderDetailsCard {...detailsData} />}
            </div>
            <div className={styles['list']}>
                <GroupOrderList onClick={(id) => loadData(id)} />
                <GroupOrderForm />
            </div>
        </div>
    );
}

export default MainPage;
