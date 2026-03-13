import { NavLink, Outlet } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import {
    LayoutDashboard,
    Car,
    Wrench,
    FileText,
    Package,
    Receipt,
    LogOut,
} from 'lucide-react';

const navItems = [
    { to: '/dashboard', label: 'Tableau de bord', icon: LayoutDashboard },
    { to: '/reception', label: 'Réception', icon: Car },
    { to: '/technician', label: 'Atelier', icon: Wrench },
    { to: '/estimates', label: 'Devis', icon: FileText },
    { to: '/inventory', label: 'Magasin', icon: Package },
    { to: '/billing', label: 'Facturation', icon: Receipt },
];

export default function AppLayout() {
    const { user, logout } = useAuth();

    return (
        <div className="app-layout">
            <aside className="sidebar">
                <div className="sidebar-header">
                    <h1 className="logo">
                        <span className="logo-icon">⚙️</span> TYREX
                    </h1>
                    <span className="logo-subtitle">Workshop Manager</span>
                </div>

                <nav className="sidebar-nav">
                    {navItems.map((item) => (
                        <NavLink
                            key={item.to}
                            to={item.to}
                            className={({ isActive }) =>
                                `nav-link ${isActive ? 'nav-link--active' : ''}`
                            }
                        >
                            <item.icon size={20} />
                            <span>{item.label}</span>
                        </NavLink>
                    ))}
                </nav>

                <div className="sidebar-footer">
                    <div className="user-info">
                        <div className="user-avatar">{user?.email?.[0]?.toUpperCase()}</div>
                        <div className="user-meta">
                            <span className="user-email">{user?.email}</span>
                            <span className="user-role">{user?.role}</span>
                        </div>
                    </div>
                    <button className="btn-logout" onClick={logout}>
                        <LogOut size={18} />
                    </button>
                </div>
            </aside>

            <main className="main-content">
                <Outlet />
            </main>
        </div>
    );
}
