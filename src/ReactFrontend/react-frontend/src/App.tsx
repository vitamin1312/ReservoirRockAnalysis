import { BrowserRouter as Router } from "react-router-dom";

import Header from "./components/Header/Header"
import Footer from "./components/Footer/Footer";

export default function App() {
  return (
    <>
    <Router>
      <div className="w-full min-h-screen bg-white">
        <Header />
        <Footer />
      </div>
    </Router>
    </>
  )
}