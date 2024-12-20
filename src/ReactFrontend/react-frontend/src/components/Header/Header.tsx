import React, { useState } from 'react';
import { Link } from 'react-router-dom';
import { FaTimes, FaBars } from "react-icons/fa";

const Header: React.FC = () => {

    const [navbar, setNavbar] = useState(false);

    const Navbar = [
        {
            name: "Home",
            link: "/",
        },
        {
            name: "About",
            link: "/about",
        },
        {
            name: "Services",
            link: "/services",
        },
    ]

    return (
        <>
        <nav className='w-full h-auto bg-blue-400 lg:px-14 md:px-14 sm:ps-12 py-2
        shadow-md'>
            <div className="justify-between mx-auto lg:w-full md:items-center md:flex">
                {/* logo & buttons */}
                <div>
                    <div className="flex items-center justify-between py-3 md:py-3 md:block">
                        {/* logo section */}
                        <Link to="/" className='text-3xl text-gray-900 font-semibold tracking-[0.1rem]'>
                          Темплея
                        </Link>
                        {/* buttons section */}
                        <div className='md:hidden'>
                            <button className="p-2 text-gray-900 rounded-md outline-none border border-transparent
                            focus:border-red-300 focus:border" onClick={() => setNavbar(!navbar)}>
                                {navbar ? (
                                    <FaTimes size={24}/>
                                ) : (
                                    <FaBars size={24}/>
                                )}
                            </button>
                        </div>
                    </div>
                </div>
                {/* menu */}
                <div className={`flex justify-between items-center md:block ${ navbar ? "block" : "hidden"}`}>
                    <ul className="list-none lg:flex md-flex sm:block block gap-x-5 gap-y-16">
                        {
                            Navbar.map((item, index) => (
                                <li key={index}>
                                    <Link to={item.link} className='text-gray-900 text-[1.15rem] font-medium
                                    tracking-wider hover:text-gray-900 ease-out duration-700'>
                                      {item.name}
                                    </Link>
                                </li>
                            ))}
                            <button className="bg-green-300 text-[1.1rem] font-normal text-gray-900
                            px-5 py-1 rounded lg:ml-10 md:ml-6 sm:ml-3 ml-3">
                                Log In
                            </button>
                    </ul>
                </div>
            </div>
        </nav>
        </>      
    );
};

export default Header;