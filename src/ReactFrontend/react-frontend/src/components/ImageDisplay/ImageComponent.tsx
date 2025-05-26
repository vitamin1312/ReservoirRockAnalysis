import React, { useState, useEffect } from 'react';

interface ImageProps {
  imageId: number;
  getImage: (imageId: number) => Promise<string>;
}

const ImageComponent: React.FC<ImageProps> = ({ imageId, getImage }) => {
  const [imageData, setImage] = useState<string | null>(null);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
  const fetchImageFile = async () => {
    try {
      const url = await getImage(imageId);
      setImage(url);
      setError(null);
    } catch (err) {
      setError(String(err));
      console.error('Error fetching file:', err);
    } finally {
      setLoading(false);
    }
  };
  fetchImageFile();
}, [imageId, getImage]);

  return (
    <>
      {loading ? (
        <p className="text-center">Загрузка...</p>
      ) : error ? (
        <p className="text-center text-red-500">{error}</p>
      ) : (
        <img
          src={imageData!}
          className="object-cover rounded-md mb-4"
          alt="Image"
        />
      )}
    </>
  );
};

export default ImageComponent;