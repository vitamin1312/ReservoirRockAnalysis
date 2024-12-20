import { BrowserRouter as Router } from "react-router-dom";
import React, { useEffect, useState } from "react";
import axios from "axios";

import Header from "./components/Header/Header";
import Footer from "./components/Footer/Footer";

export default function App() {
  const [appState, setAppState] = useState([]); // Начальное состояние как пустой массив

  useEffect(() => {
    const apiUrl = "/api/CoreSampleImages/getfromfield/1";
    axios
      .get(apiUrl, {
        headers: {
          'Content-Type': 'application/json;charset=UTF-8',
          'Access-Control-Allow-Origin': '*' // Could work and fix the previous problem, but not in all APIs
        }
        })
      .then((resp) => {
        setAppState(resp.data); // Устанавливаем данные в состояние
      })
      .catch((error) => {
        console.error("Error fetching data:", error); // Логируем ошибку
      });
  }, []);

  return (
    <>
      <Router>
        <Header />
        <div className="w-full min-h-screen bg-white p-4">
          {appState.length > 0 ? (
            <ul>
              {appState.map((item, index) => (
                <li key={index}>
                  {/* Настройте вывод в зависимости от структуры вашего объекта */}
                  <strong>json:</strong> {item.pathToImage} <br />
                  <hr />
                </li>
              ))}
            </ul>
          ) : (
            <p>Loading...</p>
          )}
        </div>
        <Footer />
      </Router>
    </>
  );
}