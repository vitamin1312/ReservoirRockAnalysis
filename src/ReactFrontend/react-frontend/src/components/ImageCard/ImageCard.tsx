import React, { useEffect, useState } from "react";
import axios from "axios";
import { ImageData } from "../../Models/ImageData";

const ImageCard: React.FC<{ data: ImageData }> = ({ data }) => {
    const [imageData, setImage] = useState<string | null>(null);
    const [loading, setLoading] = useState<boolean>(true);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        const apiUrl = `api/CoreSampleImages/getimagefile/${data.id}`;
        axios
            .get(apiUrl, { responseType: 'blob' })
            .then((resp) => {
                const url = window.URL.createObjectURL(new Blob([resp.data]));
                setImage(url);
                setLoading(false);
            })
            .catch((error) => {
                console.error("Error fetching image:", error);
                setError("Error fetching image");
                setLoading(false);
            });
    }, [data.id]);

    return (
        <div className="w-80 bg-gray-100 shadow-md rounded-lg p-4 flex flex-col justify-between">
            <h2 className="text-xl font-semibold text-gray-800 mb-2">
                {data.imageInfo.name || "Untitled"}
            </h2>
            {loading ? (
                <p className="text-center">Loading...</p>
            ) : error ? (
                <p className="text-center text-red-500">{error}</p>
            ) : (
                <img
                    src={imageData}
                    alt={data.imageInfo.name || "Image"}
                    className="w-full h-48 object-cover rounded-md mb-4"
                />
            )}
            <p className="text-gray-900 text-sm mb-2">
                {data.imageInfo.description || "Нет описания"}
            </p>
            <p className="text-gray-900 text-xs mt-auto">
                Загружено: {new Date(data.imageInfo.uploadDate).toLocaleString() || "Неизвестно"}
            </p>
        </div>
    );
};

export default ImageCard;
