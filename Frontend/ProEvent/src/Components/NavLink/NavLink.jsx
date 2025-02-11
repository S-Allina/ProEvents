import PropTypes from 'prop-types';
import './NavLink.css';
import classNames from 'classnames';
import { Link } from 'react-router-dom'; // Импортируем Link

function NavLink({ children, to, isActive }) { // Заменяем href на to
  const className = classNames('nav-item', { 'is-active': isActive });
  return (
    <Link to={to} className={className}> {/* Используем Link и to */}
      {children}
    </Link>
  );
}

NavLink.propTypes = {
  children: PropTypes.node.isRequired,
  to: PropTypes.string.isRequired, // Заменяем href на to
  isActive: PropTypes.bool,
};

NavLink.defaultProps = {
  isActive: false,
};

export default NavLink;