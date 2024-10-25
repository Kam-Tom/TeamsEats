import { createRoot } from "react-dom/client";
import { initializeIcons } from '@fluentui/react';
import App from "./components/App";
import "./index.css";

initializeIcons();

const container = document.getElementById("root");
const root = createRoot(container!);
root.render(<App />);
