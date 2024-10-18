import { useEffect, useState } from 'react';
import { app } from "@microsoft/teams-js";
import { useTeamsUserCredential } from "@microsoft/teamsfx-react";
import './App.css';

function App() {
    const { loading, teamsUserCredential } = useTeamsUserCredential({
        initiateLoginEndpoint: 'https://localhost:44302/auth-start.html',
        clientId: 'b8ae417c-389b-4e2d-8ac7-039f8ef88847',
    });

    const [message, setMessage] = useState<string | null>(null);
    const [photo, setPhoto] = useState<string | null>(null);

    useEffect(() => {
        const initializeApp = async () => {
            if (loading) {
                await app.initialize();
                app.notifySuccess();
            }
        };

        initializeApp();
    }, [loading]);

    const handleLogin = async () => {
        try {
            if (!teamsUserCredential) return;
            const token = await teamsUserCredential.getToken("");

            console.log('Token:', token);

            const response = await fetch('https://localhost:7125/Test', {
                method: 'GET',
                headers: {
                    'Authorization': `Bearer ${token!.token}`
                }
            });

            if (response.ok) {
                const data = await response.json();
                console.log(data);
                setMessage("Photo fetched successfully.");
                setPhoto(`data:image/jpeg;base64,${data.photo}`);
            } else if (response.status === 401) {
                setMessage("Unauthorized access. Please log in.");
            } else {
                setMessage(`Failed to fetch data: ${response.statusText}`);
            }
        } catch (error) {
            console.error('Error during login or fetching data:', error);
            setMessage('An error occurred. Please try again.' + error);
        }
    };
    const handleSendNotification = async () => {
        try {
            if (!teamsUserCredential) return;
            const token = await teamsUserCredential.getToken("");

            console.log('Token:', token);

            const response = await fetch('https://localhost:7125/Test/SendNotification', {
                method: 'POST',
                headers: {
                    'Authorization': `Bearer ${token!.token}`,
                    'Content-Type': 'application/json'
                }
            });

            if (response.ok) {

                setMessage("Message sent successfully.");
            } else if (response.status === 401) {
                setMessage("Unauthorized access. Please log in.");
            } else {
                setMessage(`Failed to send message: ${response.statusText}`);
            }
        } catch (error) {
            console.error('Error during sending message:', error);
            setMessage('An error occurred. Please try again.' + error);
        }
    };
    const handleSendMessage = async () => {
        try {
            if (!teamsUserCredential) return;
            const token = await teamsUserCredential.getToken("");

            console.log('Token:', token);

            const response = await fetch('https://localhost:7125/Test/SendMessage', {
                method: 'POST',
                headers: {
                    'Authorization': `Bearer ${token!.token}`,
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify("Hello, this is a test message!")
            });

            if (response.ok) {
                const data = await response.json();
                console.log(data);
                setMessage("Message sent successfully.");
            } else if (response.status === 401) {
                setMessage("Unauthorized access. Please log in.");
            } else {
                setMessage(`Failed to send message: ${response.statusText}`);
            }
        } catch (error) {
            console.error('Error during sending message:', error);
            setMessage('An error occurred. Please try again.' + error);
        }
    };

    // Add a button to trigger the send message function
    
    return (
        !loading && (
            <div>
                <h1>Welcome to the Teams App</h1>
                <button onClick={handleLogin}>Login</button>
                <button onClick={handleSendMessage}>Send Message</button>
                <button onClick={handleSendNotification}>Send Notification</button>
                {message && <p>{message}</p>}
                {photo && <img src={photo} alt="User Photo" />}
            </div>
        )
    );
}

export default App;
