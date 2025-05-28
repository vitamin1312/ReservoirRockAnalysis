import { useState, useEffect } from 'react';
import CardList from '../ImageDisplay/CardList';
import ImageTable from '../ImageDisplay/ImageTable';
import Filters from '../Filters/Filters';
import { getImagesByFilter } from '../../RestAPI/RestAPI';
import { ImageData } from '../../Models/ImageData';
import { FilterParams } from '../../Models/Filter';
import ImageInfoView from '../ImageInfo/ImageInfoView';

const MainPage: React.FC = () => {
  const isTable = localStorage.getItem('isTableView') === 'true';
  const [imagesData, setImagesData] = useState<ImageData[]>([]);
  const [isTableView, setIsTableView] = useState(isTable);
  const [filterParams, setFilterParams] = useState<FilterParams>({
    searchQuery: undefined,
    ascendingOrder: true,
    sortField: undefined,
    haveMask: undefined,
  });

  const [selectedImage, setSelectedImage] = useState<ImageData | null>(null);

  const fetchImages = async () => {
    try {
      const data = await getImagesByFilter(filterParams);
      setImagesData(data);
    } catch (error) {
      console.error("Error fetching images:", error);
      setImagesData([]);
    } finally {
    }
  };

  const refreshImages = () => {
    fetchImages();
  };

  const handleImageClick = (image: ImageData) => {
    setSelectedImage(image);
  };

  useEffect(() => {
    fetchImages();
  }, []);

  const toggleView = () => {
    const newView = !isTableView;
    setIsTableView(newView);
    localStorage.setItem('isTableView', String(newView));
    setFilterParams({ ...filterParams });
  };

  return (
    <div className="flex flex-col h-full overflow-hidden">
      <div className="flex flex-grow overflow-hidden">
        <div className="w-1/6 bg-gray-100 p-4 overflow-y-auto h-full">
          <Filters
            onToggleView={toggleView}
            isTableView={isTableView}
            filterParams={filterParams}
            setFilterParams={setFilterParams}
            onApplyFilters={fetchImages}
          />
        </div>
        <div className="w-1/4 bg-gray-200 overflow-y-auto h-full">
          {isTableView ? (
            <ImageTable imageDataList={imagesData} onImageClick={handleImageClick} />
          ) : (
            <CardList imageDataList={imagesData} onImageClick={handleImageClick} />
          )}
        </div>
        <div className="w-7/12 bg-gray-100 flex flex-col h-full overflow-y-auto">
          <ImageInfoView
            image={selectedImage}
            fetchImages={refreshImages}
          />
        </div>
      </div>
    </div>
  );
};

export default MainPage;
