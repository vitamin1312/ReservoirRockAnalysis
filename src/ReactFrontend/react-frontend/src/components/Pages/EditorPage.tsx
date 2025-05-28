import { useEffect, useState } from 'react';
import DrawingCanvas from '../Editor/GraphicEditor'
import { getImageUrl, getMaskImageUrl } from '../../RestAPI/RestAPI';
import { useParams } from 'react-router-dom';
import {removeBlackBackground} from '../../Utils/utils'

const EditorPage = () => {
  const { id } = useParams<{ id: string }>();
  const [backgroundImage, setBackgroundImage] = useState<string | null>(null);
  const [editableImage, setEditableImage] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    if (!id) return;

    const fetchImages = async () => {
      setLoading(true);
      const imageId = Number(id);

      try {
        const backgroundImageUrl = await getImageUrl(imageId);
        const backgroundImg = new Image();
        backgroundImg.crossOrigin = 'anonymous';
        backgroundImg.src = backgroundImageUrl;

        backgroundImg.onload = async () => {
          setBackgroundImage(backgroundImageUrl);

          try {
            const editableImageUrl = await getMaskImageUrl(imageId);
            const editableImg = new Image();
            editableImg.crossOrigin = 'anonymous';
            editableImg.src = editableImageUrl;

            editableImg.onload = () => {
              const canvas = removeBlackBackground(editableImg);
              const processedUrl = canvas.toDataURL();
              setEditableImage(processedUrl);
              setLoading(false);
            };

            editableImg.onerror = (e) => {
              console.error("Ошибка загрузки editableImage", e);
              // ← создаём пустую маску
              const fallback = document.createElement("canvas");
              fallback.width = backgroundImg.width;
              fallback.height = backgroundImg.height;
              setEditableImage(fallback.toDataURL());
              setLoading(false);
            };
          } catch (e) {
            console.error("Ошибка при получении editableImage:", e);
            // ← создаём пустую маску
            const fallback = document.createElement("canvas");
            fallback.width = backgroundImg.width;
            fallback.height = backgroundImg.height;
            setEditableImage(fallback.toDataURL());
            setLoading(false);
          }
        };

        backgroundImg.onerror = () => {
          console.error("Ошибка загрузки backgroundImage");
          setLoading(false);
        };
      } catch (e) {
        console.error("Ошибка загрузки изображений", e);
        setLoading(false);
      }
    };

  fetchImages();
}, [id]);
  if (loading) return <div className="p-4">Загрузка...</div>;

  return (
    <div className="h-screen">
      {id && backgroundImage && editableImage ? (
        <DrawingCanvas
          objectId={Number(id)}
          backgroundImageSrc={backgroundImage}
          editableImageSrc={editableImage}
        />
      ) : (
        <div className="flex items-center justify-center h-full text-gray-500">
          Загрузка...
        </div>
      )}
    </div>
  );
};

export default EditorPage;