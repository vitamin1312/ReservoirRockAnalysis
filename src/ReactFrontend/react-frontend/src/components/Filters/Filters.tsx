import React, { useState, useEffect } from 'react';
import Dropdown from '../UI/Dropdown';
import { FilterParams } from '../../Models/Filter';
import { getAllFields } from '../../RestAPI/RestAPI';
import { FieldData } from '../../Models/ImageData';
import RadioGroup from '../UI/RadioGroup';

interface FiltersProps {
  onToggleView: () => void;
  isTableView: boolean;
  filterParams: FilterParams;
  setFilterParams: React.Dispatch<React.SetStateAction<FilterParams>>;
  onApplyFilters: () => void;
}

const Filters: React.FC<FiltersProps> = ({
  onToggleView,
  isTableView,
  filterParams,
  setFilterParams,
  onApplyFilters
}) => {

  const handleMaskFilterChange = (value: 'all' | 'withMask' | 'withoutMask') => {
    const haveMask = value === 'withMask' ? true : value === 'withoutMask' ? false : undefined;
    setFilterParams({ ...filterParams, haveMask });
  };

  const radioOptions = [
    { value: 'all', label: 'Все изображения' },
    { value: 'withMask', label: 'С маской' },
    { value: 'withoutMask', label: 'Без маски' },
  ];

  const allFields: FieldData = {
    id: -1,
    name: 'Все месторождения',
    description: 'Все месторождения'
  }

  const handleApplyFilters = () => {
    onApplyFilters();
  };

  const [fieldsData, setFieldsData] = useState<Array<FieldData>>([]);

  useEffect(() => {
    const fetchImages = async () => {
      try {
        const data = await getAllFields();
        setFieldsData(data);
      } catch (error) {
        console.error('Error fetching images:');
      }
    };

    fetchImages();
  }, []);

  return (
    <div className='h-full flex flex-col'>
      <div className="p-4 flex flex-row">
        <div className="flex items-center space-x-2">
          <p className="text-gray-900">Таблица</p>
          <input
            type="checkbox"
            checked={isTableView}
            onChange={onToggleView}
            className="form-checkbox h-5 w-5 text-green-300"
          />
        </div>
      </div>

      <div className='p-4 items-center border-t'>
        <form className="flex flex-col space-y-4">
          <input
            type="query"
            placeholder="Поиск"
            className="p-2 border rounded-md focus:outline-none focus:ring-2 focus:ring-blue-400"
            required
            onChange={(e) => {
            setFilterParams({ ...filterParams, searchQuery: e.target.value || '' });
              console.log(e.target.value);
            }
            }
          />
        </form>
      </div>

      <div className="p-4 flex flex-row items-center">
        <div className="flex items-center space-x-2">
          <p className="text-gray-900">По возрастанию</p>
          <input
            type="checkbox"
            defaultChecked={true}
            className="form-checkbox h-5 w-5 text-green-300"
            onChange={(e) =>
              setFilterParams({ ...filterParams, ascendingOrder: e.target.checked })
            }
          />
        </div>
      </div>

      <div className="p-4">
        <RadioGroup
          options={radioOptions}
          name="filter"
          selectedValue={filterParams.haveMask === true ? 'withMask' : filterParams.haveMask === false ? 'withoutMask' : 'all'}
          onChange={(value) => handleMaskFilterChange(value as 'all' | 'withMask' | 'withoutMask')}
        />
      </div>

      <div className="p-4 flex flex-row items-center">
        <div className="flex items-center space-x-2">
        <Dropdown
          label="Месторождение"
          placeholder="Название"
          options={[allFields].concat(fieldsData)}
          onSelect={(value) => setFilterParams({ ...filterParams, sortField: value })}
          selectedId={filterParams.sortField || -1}
        />

        </div>
      </div>

      <div className='flex-grow'></div>

      <button
        type="submit"
        className="bg-green-300 text-gray-900 py-3 rounded-md hover:bg-green-400 p-3"
        onClick={handleApplyFilters}
      >Применить</button>
    </div>
  );
}

export default Filters;