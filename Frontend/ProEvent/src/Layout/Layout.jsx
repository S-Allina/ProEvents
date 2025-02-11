import PropTypes from 'prop-types';
import Header from '../Components/Header/Header';
import './Layout.css'; // Общий стиль для App

const Layout = ({ children }) => {
  return (
    <div className="app">
      <Header />
      <main>{children}</main>
    </div>
  );
};

Layout.propTypes = {
  children: PropTypes.node.isRequired, // Или PropTypes.element.isRequired, если ожидается один элемент
};

export default Layout;
