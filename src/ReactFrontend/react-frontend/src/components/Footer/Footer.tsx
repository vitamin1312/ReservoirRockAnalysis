import React from 'react';

const Footer: React.FC = () => {

    return (
        <>
        <div className="sticky bottom-0" >
        <footer className="h-6 p-6 text-gray-900 bg-blue-400 text-xl flex justify-between items-center">
            <div>
                Summary info
            </div>
            <div className='font-semibold text-right'>
                РГУ нефти и газа (НИУ) имени И.М. Губкина
            </div>
            </footer>   
        </div>
        </>
    );
};

export default Footer;