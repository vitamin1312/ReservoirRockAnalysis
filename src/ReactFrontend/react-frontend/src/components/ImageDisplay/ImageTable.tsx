import React from 'react';
import { ImageData } from '../../Models/ImageData';

const ImageTable: React.FC<{ imageDataList: Array<ImageData> }> = ({ imageDataList }) => {
  return (
    <div className="flex flex-col h-full">
      <div className="flex-grow overflow-y-auto bg-white p-4">
        {imageDataList.length > 0 ? (
          <table className="min-w-full bg-white w-full table-fixed">
            <thead>
              <tr>
                <th className="py-2 px-4 border-b w-1/4">Название</th>
                <th className="py-2 px-4 border-b w-1/4">Описание</th>
                <th className="py-2 px-4 border-b w-1/4">Время загрузки</th>
              </tr>
            </thead>
            <tbody>
              {imageDataList.map((item, index) => (
                <tr key={index} className="border-b">
                  <td className="py-2 px-4 truncate">{item.imageInfo.name || "Untitled"}</td>
                  <td className="py-2 px-4 truncate">{item.imageInfo.description || "Нет описания"}</td>
                  <td className="py-2 px-4">
                    {new Date(item.imageInfo.uploadDate).toLocaleString() || "Неизвестно"}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        ) : (
          <p className="text-center">Не удалось загрузить данные</p>
        )}
      </div>
    </div>
  );
};

export default ImageTable;
