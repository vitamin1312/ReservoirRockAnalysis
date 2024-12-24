import React, {useState, useEffect} from 'react';
import { ImageData } from '../../Models/ImageData';
import ImageConvert from './ImageConvert';
import { getImageUrl } from '../../RestAPI/RestAPI';

interface ImageInfoProps {
  image: ImageData | null;
}

const ImageInfo: React.FC<ImageInfoProps> = ({ image }) => {
  if (!image) {
    return <div>Выберите изображение для просмотра информации.</div>;
  }

  const [imageData, setImage] = useState<string | null>(null);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
      const fetchImageFile = async () => {
          try {
              const url = await getImageUrl(image.id)
              setImage(url);
              setLoading(false);
          } catch (err) {
              setError(String(err));
              console.error('Error fetching file:', err);
          }
      };
      fetchImageFile();
      }, [image.id]);

  return (
    <>
    <div className="inline-block w-1/2 h-auto m-1">
                {loading ? (
                    <p className="text-center">Loading...</p>
                ) : error ? (
                    <p className="text-center text-red-500">{error}</p>
                ) : (
                    <img
                        src={imageData || "Loading"}
                        alt={image.imageInfo.name || "Image"}
                        className="object-cover rounded-md mb-4"
                    />
                )}
            </div>
      <div className="flex-grow bg-gray-100 p-4 overflow-auto">
      <h3>Информация</h3>
      <div>
        <label>
          Название:
          <input
            type="text"
            value={image.imageInfo.name}
          />
        </label>
      </div>
      <div>
        <label>
          Описание:
          <textarea
            value={image.imageInfo.description}
          />
        </label>
      </div>
      <div>
        <label>
          Дата загрузки:
          <input
            type="text"
            value={image.imageInfo.uploadDate}
            disabled
          />
        </label>
      </div>
      <div>
        <label>
          Месторождение:
          <select
            value={image.imageInfo.fieldId}
          >
          </select>
        </label>
      </div>
      <button>Сохранить</button>
    </div>
    <ImageConvert />
  </>
    
  );
};

export default ImageInfo;