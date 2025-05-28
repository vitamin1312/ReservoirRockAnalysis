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

    // Имитируем API-запрос
  const fetchImages = async () => {
    setLoading(true);
    try {
      let imageId = Number(id);
      let backgroundImageUrl = await getImageUrl(imageId);
      let editableImageUrl = await getMaskImageUrl(imageId);

      // Обработка переднего плана
      const img = new Image();
      img.crossOrigin = 'anonymous'; // если нужно
      img.src = editableImageUrl;

      img.onload = () => {
        const canvas = removeBlackBackground(img);
        const processedUrl = canvas.toDataURL();

        setBackgroundImage(backgroundImageUrl);
        setEditableImage(processedUrl);
        setLoading(false);
      };

      img.onerror = () => {
        console.error("Ошибка загрузки editableImage");
        setLoading(false);
      };
    } catch (e) {
      console.error('Ошибка загрузки изображений', e);
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