import React, { useEffect, useState } from "react";
import { ImageData } from "../../Models/ImageData";
import { getImageFile } from "../../RestAPI/RestAPI";

const ImageCard: React.FC<{ data: ImageData }> = ({ data }) => {
    const [imageData, setImage] = useState<string | null>(null);
    const [loading, setLoading] = useState<boolean>(true);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        const fetchImageFile = async () => {
            try {
                const file = await getImageFile(data.id);
                const url = window.URL.createObjectURL(new Blob([file]));
                setImage(url);
                setLoading(false);
            } catch (err) {
                setError(String(err));
                console.error('Error fetching file:', err);
            }
        };
        fetchImageFile();
    }, [data.id]);

    return (
        <div className="w-96 bg-gray-100 shadow-md rounded-lg p-0.5 m-0.5 flex flex-row flex-auto justify-center items-center">
            <div className="inline-block w-1/2 h-auto m-1">
                {loading ? (
                    <p className="text-center">Loading...</p>
                ) : error ? (
                    <p className="text-center text-red-500">{error}</p>
                ) : (
                    <img
                        src={imageData || "Loading"}
                        alt={data.imageInfo.name || "Image"}
                        className="object-cover rounded-md mb-4"
                    />
                )}
            </div>
            <div className="inline-block m-1 w-1/2">
                <h2 className="text-xl font-semibold text-gray-900 mb-2 truncate overflow-hidden">
                    {data.imageInfo.name || "Untitled"}
                </h2>
                <p className="text-gray-900 text-sm mb-2 truncate overflow-hidden">
                    {data.imageInfo.description || "Нет описания"}
                </p>
                <p className="text-gray-900 text-xs mt-auto truncate overflow-hidden">
                    Загружено: {new Date(data.imageInfo.uploadDate).toLocaleString() || "Неизвестно"}
                </p>
            </div>
        </div>
    );
};

export default ImageCard;
