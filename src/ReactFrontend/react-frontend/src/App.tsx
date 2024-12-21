import React, { useState, useEffect } from 'react';
import { BrowserRouter as Router } from 'react-router-dom';
import axios from 'axios';
import Header from './components/Header/Header';
import Footer from './components/Footer/Footer';
import CardList from './components/ImageDisplay/CardList';
import ImageTable from './components/ImageDisplay/ImageTable';
import Filters from './components/Filters/Filters';

export default function App() {

  const [appState, setAppState] = useState([]);
  const isTable = localStorage.getItem('isTableView') === 'true';
  const [isTableView, setIsTableView] = useState(isTable);

  useEffect(() => {
    const apiUrl = '/api/CoreSampleImages/getfromfield/1';
    axios
      .get(apiUrl)
      .then((resp) => {
        setAppState(resp.data);
      })
      .catch((error) => {
        console.error('Error fetching data:', error);
      });
  }, []);

  const toggleView = () => {
    const newView = !isTableView;
    setIsTableView(newView);
    localStorage.setItem('isTableView', String(newView));
  };

  return (
    <div className="flex flex-col h-screen">
      <Router>
        <Header />
        <div className="flex flex-grow overflow-hidden">
          <div className="w-1/12 bg-gray-100 p-4">
            <Filters onToggleView={toggleView} isTableView={isTableView} />
          </div>
          <div className="w-1/4 bg-gray-200 overflow-y-auto">
          {isTableView ? (
              <ImageTable imageDataList={appState} />
            ) : (
              <CardList imageDataList={appState} />
            )}
          </div>
          <div className="w-2/3 bg-gray-300 overflow-hidden flex flex-col">
            <div className="flex-grow bg-gray-400 p-4">
              Image info
            </div>
            <div className="h-16 bg-gray-500 p-4">
              Image convert
            </div>
          </div>
        </div>
        <Footer />
      </Router>
    </div>
  );
}
