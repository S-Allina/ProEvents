import './Loader.css'; // Импортируйте CSS файл


const Loader = () => {

  return (
    <div className='Container'>
    <div className="loader">
    <div className="load-inner load-one"></div>
    <div className="load-inner load-two"></div>
    <div className="load-inner load-three"></div>
    <span className="text">Loading...</span>
  </div>
  </div>
  );
};

export default Loader;