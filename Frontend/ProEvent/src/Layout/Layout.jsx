import PropTypes from 'prop-types';
import Header from '../Components/Header/Header';
import './Layout.css';
const Layout = ({ children }) => {
  return (
    <div className="app">
      <Header />
      <main>{children}</main>
    </div>
  );
};

Layout.propTypes = {
  children: PropTypes.node.isRequired,
};

export default Layout;
