import React from 'react';
import { ImageData } from '../../Models/ImageData';

const ImageTable: React.FC<{ imageDataList: Array<ImageData> }> = ({ imageDataList }) => {
  return (
    <div className="flex flex-col h-full">
        <div className="flex-grow overflow-y-auto bg-white p-4">
      {imageDataList.length > 0 ? (
        <ul className="flex flex-wrap justify-center">
            <table className="min-w-full bg-white">
            <thead>
              <tr>
                <th className="py-2 px-4 border-b">Название</th>
                <th className="py-2 px-4 border-b">Описание</th>
                <th className="py-2 px-4 border-b">Время загрузки</th>
              </tr>
            </thead>
            <tbody>
              {imageDataList.map((item, index) => (
                <tr key={index} className="border-b">
                  <td className="py-2 px-4">{item.imageInfo.name || "Untitled"}</td>
                  <td className="py-2 px-4">{item.imageInfo.description || "Нет описания"}</td>
                  <td className="py-2 px-4">
                    {new Date(item.imageInfo.uploadDate).toLocaleString() || "Неизвестно"}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </ul>
      ) : (
        <p className="text-center">Не удалось загрузить данные</p>
      )}
    </div>
    </div>
    
  );
};

export default ImageTable;