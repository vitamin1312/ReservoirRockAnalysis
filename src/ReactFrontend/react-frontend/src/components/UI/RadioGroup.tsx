import React from 'react';
import { useEffect } from 'react';

interface RadioOption {
  value: string;
  label: string;
}

interface RadioGroupProps {
  options: RadioOption[];
  name: string;
  selectedValue: string;
  onChange: (value: string) => void;
}

const RadioGroup: React.FC<RadioGroupProps> = ({
  options,
  name,
  selectedValue,
  onChange,
}) => {
    useEffect(() => {
        if (!selectedValue && options.length > 0) {
          onChange(options[0].value);
        }
      }, [selectedValue, options, onChange]);
    return (
        <fieldset className="space-y-4">
            {options.map((option) => (
            <div key={option.value}>
                <label className="flex items-center space-x-2">
                <input
                    type="radio"
                    name={name}
                    value={option.value}
                    checked={selectedValue === option.value}
                    onChange={() => onChange(option.value)}
                    className="form-radio text-blue-500"
                />
                <span>{option.label}</span>
                </label>
            </div>
            ))}
        </fieldset>
    );
};

export default RadioGroup;