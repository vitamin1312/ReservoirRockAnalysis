import React from 'react';
import { ImageData } from '../../Models/ImageData'
import ImageCard from './ImageCard';

const CardList: React.FC<{ imageDataList: Array<ImageData> }> = ({ imageDataList }) => {
  return (
    <div className="flex flex-col h-full">
      <div className="flex-grow overflow-y-auto bg-white p-4">
        {imageDataList.length > 0 ? (
          <ul className="flex flex-wrap justify-center">
            {imageDataList.map((item, index) => (
              <li key={index} className="mb-4">
                <ImageCard data={item} />
              </li>
            ))}
          </ul>
        ) : (
          <p className="text-center">Не удалось загрузить данные</p>
        )}
      </div>
    </div>
  );
};

export default CardList;
