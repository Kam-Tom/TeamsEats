import React, { useContext } from "react";
import { useParams } from "react-router-dom";
import DetailsPage from './Details/DetailsPage';
import ListPage from './List/ListPage';
import { TeamsFxContext } from "./Context";

const Tab: React.FC = () => {
    const { themeString } = useContext(TeamsFxContext);
    const { id } = useParams<{ id?: string }>();

    return (
        <div
            className={themeString === "default" ? "light" : themeString === "dark" ? "dark" : "contrast"}
        >
            {id ? <DetailsPage id={id} /> : <ListPage />}
        </div>
    );
}

export default Tab;
