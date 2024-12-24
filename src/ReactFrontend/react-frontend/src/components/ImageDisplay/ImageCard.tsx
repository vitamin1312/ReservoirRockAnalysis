import React from "react";
import { ImageData } from "../../Models/ImageData";
import { getImageUrl } from "../../RestAPI/RestAPI";
import ImageComponent from "./ImageComponent";

const ImageCard: React.FC<{ data: ImageData }> = ({ data }) => {




    return (
        <div className="w-96 bg-gray-100 shadow-md rounded-lg p-0.5 m-0.5 flex flex-row flex-auto justify-center items-center">
            <div className="inline-block w-1/2 h-auto m-1">
                <ImageComponent imageId={data.id}
                getImage = {getImageUrl}/>
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
