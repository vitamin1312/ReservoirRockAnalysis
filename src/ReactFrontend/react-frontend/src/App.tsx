import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import Header from './components/Header/Header';
import Footer from './components/Footer/Footer';
import MainPage from './components/Pages/MainPage';
import About from './components/Pages/About';
import NotFound from './components/Pages/NotFound';

export default function App() {
  return (
    <Router>
      <div className="flex flex-col h-screen">
        <Header />
        <div className="flex-grow overflow-hidden">
          <Routes>
            <Route path="/" element={<MainPage />} />
            <Route path="about" element={<About />} />
            <Route path="*" element={<NotFound />} />
          </Routes>
        </div>
        <Footer />
      </div>
    </Router>
  );
}
