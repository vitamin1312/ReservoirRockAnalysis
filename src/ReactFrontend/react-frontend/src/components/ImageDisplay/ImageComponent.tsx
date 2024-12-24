import React, { useEffect, useState } from "react";

interface ImageProps {
  imageId: number;
  getImage: (imageId: number) => Promise<string>
}

const ImageComponent: React.FC<ImageProps> = ({ imageId, getImage }) => {
    const [imageData, setImage] = useState<string | null>(null);
    const [loading, setLoading] = useState<boolean>(true);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        const fetchImageFile = async () => {
            try {
                const url = await getImage(imageId)
                setImage(url);
                setLoading(false);
            } catch (err) {
                setError(String(err));
                console.error('Error fetching file:', err);
            }
        };
        fetchImageFile();
    }, [imageId]);

    return (
        <>
        {loading ? (
            <p className="text-center">Loading...</p>
        ) : error ? (
            <p className="text-center text-red-500">{error}</p>
        ) : (
            <img
                src={imageData || "Loading"}
                className="object-cover rounded-md mb-4"
            />
        )}
        </>
    );
};

export default ImageComponent;
