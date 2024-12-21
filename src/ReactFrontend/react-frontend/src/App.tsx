import React, { useState, useEffect } from 'react';
import { BrowserRouter as Router } from 'react-router-dom';
import axios from 'axios';
import Header from './components/Header/Header';
import Footer from './components/Footer/Footer';
import CardList from './components/ImageDisplay/CardList';
import ImageTable from './components/ImageDisplay/ImageTable';
import Filters from './components/Filters/Filters';
import { getImagesFromField } from './RestAPI/RestAPI';
import { ImageData } from './Models/ImageData';

export default function App() {

  const isTable = localStorage.getItem('isTableView') === 'true';

  const [imagesData, setImagesData] = useState(Array<ImageData>);
  const [isTableView, setIsTableView] = useState(isTable);
  const FieldId = 1;

  useEffect(() => {
    const fetchImages = async () => {
      try {
        const data = await getImagesFromField(1);
        setImagesData(data);
      } catch (error) {
        console.error('Error fetching images:', FieldId);
      }
    };

    fetchImages();
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
              <ImageTable imageDataList={imagesData} />
            ) : (
              <CardList imageDataList={imagesData} />
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
