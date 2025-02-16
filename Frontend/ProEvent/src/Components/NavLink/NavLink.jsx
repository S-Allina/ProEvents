import PropTypes from 'prop-types';
import './NavLink.css';
import classNames from 'classnames';
import { Link } from 'react-router-dom';

function NavLink({ children, to, isActive }) {
  const className = classNames('nav-item', { 'is-active': isActive });
  return (
    <Link to={to} className={className}>
      {children}
    </Link>
  );
}

NavLink.propTypes = {
  children: PropTypes.node.isRequired,
  to: PropTypes.string.isRequired,
  isActive: PropTypes.bool,
};

NavLink.defaultProps = {
  isActive: false,
};

export default NavLink;
