import React from "react";
import {
    FluentProvider,
    teamsLightTheme,
    teamsDarkTheme,
    teamsHighContrastTheme,
    Spinner,
    tokens,
} from "@fluentui/react-components";
import { HashRouter as Router, Navigate, Route, Routes } from "react-router-dom";
import { useTeamsUserCredential } from "@microsoft/teamsfx-react";
import Privacy from "./Privacy";
import TermsOfUse from "./TermsOfUse";
import Tab from "./Tab";
import { TeamsFxContext } from "./Context";
import config from "./lib/config";

const App: React.FC = () => {
    const { loading, theme, themeString, teamsUserCredential } = useTeamsUserCredential({
        initiateLoginEndpoint: config.initiateLoginEndpoint!,
        clientId: config.clientId!,
    });

    return (
        <TeamsFxContext.Provider value={{ theme, themeString, teamsUserCredential }}>
            <FluentProvider
                theme={
                    themeString === "dark"
                        ? teamsDarkTheme
                        : themeString === "contrast"
                            ? teamsHighContrastTheme
                            : {
                                ...teamsLightTheme,
                                colorNeutralBackground3: "#eeeeee",
                            }
                }
                style={{ background: tokens.colorNeutralBackground3 }}
            >
                <Router>
                    {loading ? (
                        <Spinner style={{ margin: 100 }} />
                    ) : (
                        <Routes>
                            <Route path="/privacy" element={<Privacy />} />
                            <Route path="/termsofuse" element={<TermsOfUse />} />
                            <Route path="/tab" element={<Tab />} />
                            <Route path="/tab/:id" element={<Tab />} />
                            <Route path="*" element={<Navigate to={"/tab"} />} />
                        </Routes>
                    )}
                </Router>
            </FluentProvider>
        </TeamsFxContext.Provider>
    );
}

export default App;
