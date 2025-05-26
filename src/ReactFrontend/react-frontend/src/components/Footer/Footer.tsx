import React from 'react';
import { useLocation } from 'react-router-dom';

const pages: Record<string, string> = {
  "/": "Главная страница",
  "/upload": "Загрузить",
  "/about": "О приложении"
};

const Footer: React.FC = () => {
  const location = useLocation();
  const currentLocation = String(location.pathname)
  const pageTitle = pages[currentLocation] || "Страница";

  return (
    <footer className="w-full shrink-0 h-16 p-4 text-gray-900 bg-blue-400 text-xl flex justify-between items-center">
      <div>{pageTitle}</div>
      <div className="font-semibold text-right">РГУ нефти и газа (НИУ) имени И.М. Губкина</div>
    </footer>
  );
};

export default Footer;