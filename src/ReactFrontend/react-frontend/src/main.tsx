import React from "react";
import ReactDOM from "react-dom/client";
import App from "./App";
import Modal from "react-modal";

Modal.setAppElement("#root");

ReactDOM.createRoot(document.getElementById("root") as HTMLElement).render(
  <React.StrictMode>
    <App />
  </React.StrictMode>
);