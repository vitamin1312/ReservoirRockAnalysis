import { useState, useEffect } from 'react';
import CardList from '../ImageDisplay/CardList';
import ImageTable from '../ImageDisplay/ImageTable';
import Filters from '../../Filters/Filters';
import { getAllImages } from '../../RestAPI/RestAPI';
import { ImageData } from '../../Models/ImageData';

const MainPage: React.FC = () => {
  const isTable = localStorage.getItem('isTableView') === 'true';
  const [imagesData, setImagesData] = useState<ImageData[]>([]);
  const [isTableView, setIsTableView] = useState(isTable);

  useEffect(() => {
    const fetchImages = async () => {
      try {
        const data = await getAllImages();
        setImagesData(data);
      } catch (error) {
        console.error('Error fetching images:');
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
    <div className="flex flex-col h-full overflow-hidden">
      <div className="flex flex-grow overflow-hidden">
        <div className="w-1/12 bg-gray-100 p-4">
          <Filters onToggleView={toggleView} isTableView={isTableView} />
        </div>
        <div className="w-1/4 bg-gray-200 overflow-y-auto h-full">
          {isTableView ? (
            <ImageTable imageDataList={imagesData} />
          ) : (
            <CardList imageDataList={imagesData} />
          )}
        </div>
        <div className="w-2/3 bg-gray-300 flex flex-col h-full">
          <div className="flex-grow bg-gray-400 p-4 overflow-auto">
            Image info
          </div>
          <div className="h-16 bg-gray-500 p-4">
            Image convert
          </div>
        </div>
      </div>
    </div>
  );
};

export default MainPage;
