import React, { useEffect, useState } from 'react';
import { makeStyles } from '@fluentui/react-components';

const useStyles = makeStyles({
    red: {
        color: 'red',
    },
});

interface TimerProps {
    targetTime: Date;
}

const Timer: React.FC<TimerProps> = ({ targetTime }) => {
    const classes = useStyles();

    const [minutes, setMinutes] = useState(0);
    const [hours, setHours] = useState(0);

    const getTime = () => {
        const time = new Date(targetTime).getTime() - Date.now();
        const h = Math.max(Math.floor((time / (1000 * 60 * 60)) % 24), 0);
        const m = Math.max(Math.floor((time / 1000 / 60) % 60), 0);
        setHours(h);
        setMinutes(m);
    };

    useEffect(() => {
        getTime();
        const interval = setInterval(() => getTime(), 1000);

        return () => clearInterval(interval);
    }, [targetTime]);

    const formattedHours = String(hours).padStart(2, '0');
    const formattedMinutes = String(minutes).padStart(2, '0');

    return (
        <div className={`${hours <= 0 ? classes.red : ''}`}>
            {`${formattedHours}:${formattedMinutes}`}
        </div>
    );
};

export default Timer;
