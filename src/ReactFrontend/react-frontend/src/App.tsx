import { BrowserRouter as Router } from "react-router-dom";
import { useEffect, useState } from "react";
import axios from "axios";

import Header from "./components/Header/Header";
import Footer from "./components/Footer/Footer";
import ImageCard from "./components/ImageCard/ImageCard";

export default function App() {
  const [appState, setAppState] = useState([]);

  useEffect(() => {
    const apiUrl = "/api/CoreSampleImages/getfromfield/2";
    axios
      .get(apiUrl)
      .then((resp) => {
        setAppState(resp.data);
      })
      .catch((error) => {
        console.error("Error fetching data:", error);
      });
  }, []);

  return (
    <>
    <div className="flex flex-col min-h-screen">
      <Router>
        <Header />
        <div className="w-full flex-grow bg-white p-4">
          {appState.length > 0 ? (
            <ul>
              {appState.map((item, index) => (
                <li key={index}>
                  <ImageCard data={item} />
                </li>
              ))}
            </ul>
          ) : (
            <p>Не удалось загрузить данные</p>
          )}
        </div>
        <Footer />
      </Router>
      </div>
    </>
  );
}