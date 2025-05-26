import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import Header from './components/Header/Header';
import Footer from './components/Footer/Footer';
import MainPage from './components/Pages/MainPage';
import ImageUploadPage from './components/Pages/Upload'
import About from './components/Pages/About';
import NotFound from './components/Pages/NotFound';
import { useEffect } from "react";
import { setAuthModalOpener } from "./RestAPI/RestAPI";

import { AuthModalProvider, useAuthModal } from "./components/Auth/AuthModalContext";
import AuthModal from "./components/Auth/Auth";

const AuthModalManager = () => {
  const { isOpen, closeModal } = useAuthModal();
  return <AuthModal isOpen={isOpen} onClose={closeModal} />;
};

const AxiosInterceptorInitializer = () => {
  const { openModal } = useAuthModal();

  useEffect(() => {
    setAuthModalOpener(openModal); // Устанавливаем глобальный вызов
  }, [openModal]);

  return null;
};

export default function App() {
  return (
    <AuthModalProvider>
      <Router>
        <div className="flex flex-col h-screen">
          <Header />
          <div className="flex-grow overflow-hidden">
            <Routes>
              <Route path="/" element={<MainPage />} />
              <Route path="about" element={<About />} />
              <Route path="upload" element={<ImageUploadPage />} />
              <Route path="*" element={<NotFound />} />
            </Routes>
          </div>
          <Footer />
        </div>
        <AuthModalManager />
        <AxiosInterceptorInitializer />
      </Router>
    </AuthModalProvider>
  );
}
