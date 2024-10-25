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
        const interval = setInterval(() => getTime(), 60000);

        return () => clearInterval(interval);
    }, [targetTime]);


    return (
        <div className={`${hours <= 0 ? classes.red : ''}`}>
            {`${hours}:${minutes}`}
        </div>
    );
};

export default Timer;