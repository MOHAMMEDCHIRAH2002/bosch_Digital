import { useEffect, useState } from 'react';
import { dashboardApi } from '../api/endpoints';
import { ClipboardCheck, FileText, Car, DollarSign } from 'lucide-react';

interface Kpis {
    activeRepairOrders: number;
    pendingEstimates: number;
    vehiclesReadyForPickup: number;
    todayRevenue: number;
}

interface ActiveOrder {
    id: string;
    orderNumber: string;
    customerName: string;
    vehicleDetails: string;
    status: string;
    createdAtUtc: string;
}

const statusBadgeMap: Record<string, string> = {
    Diagnosis: 'badge--info',
    AwaitingCustomerApproval: 'badge--warning',
    Approved: 'badge--success',
    RepairInProgress: 'badge--primary',
    RepairCompleted: 'badge--success',
    QualityValidated: 'badge--success',
    Invoiced: 'badge--info',
    Delivered: 'badge--success',
};

export default function DashboardPage() {
    const [kpis, setKpis] = useState<Kpis | null>(null);
    const [orders, setOrders] = useState<ActiveOrder[]>([]);

    useEffect(() => {
        dashboardApi.getKpis().then((r) => setKpis(r.data)).catch(console.error);
        dashboardApi.getActiveOrders().then((r) => setOrders(r.data)).catch(console.error);
    }, []);

    return (
        <div>
            <div className="page-header">
                <h2>Tableau de bord</h2>
                <p>Vue d'ensemble de l'atelier</p>
            </div>

            {/* KPI Cards */}
            <div className="card-grid">
                <div className="card kpi-card kpi--primary">
                    <span className="kpi-label"><ClipboardCheck size={14} /> OR Actifs</span>
                    <span className="kpi-value">{kpis?.activeRepairOrders ?? '—'}</span>
                </div>
                <div className="card kpi-card kpi--warning">
                    <span className="kpi-label"><FileText size={14} /> Devis en attente</span>
                    <span className="kpi-value">{kpis?.pendingEstimates ?? '—'}</span>
                </div>
                <div className="card kpi-card kpi--success">
                    <span className="kpi-label"><Car size={14} /> Prêts à livrer</span>
                    <span className="kpi-value">{kpis?.vehiclesReadyForPickup ?? '—'}</span>
                </div>
                <div className="card kpi-card kpi--info">
                    <span className="kpi-label"><DollarSign size={14} /> CA du jour</span>
                    <span className="kpi-value">{kpis ? `${kpis.todayRevenue.toFixed(2)} €` : '—'}</span>
                </div>
            </div>

            {/* Active Orders Table */}
            <div className="card mt-3">
                <div className="section-heading">
                    <h3>Ordres de réparation actifs</h3>
                </div>
                <table className="data-table">
                    <thead>
                        <tr>
                            <th>N° OR</th>
                            <th>Client</th>
                            <th>Véhicule</th>
                            <th>Statut</th>
                            <th>Créé le</th>
                        </tr>
                    </thead>
                    <tbody>
                        {orders.length === 0 ? (
                            <tr>
                                <td colSpan={5} style={{ textAlign: 'center', color: 'var(--clr-text-dim)' }}>
                                    Aucun ordre de réparation actif
                                </td>
                            </tr>
                        ) : (
                            orders.map((order) => (
                                <tr key={order.id}>
                                    <td style={{ fontWeight: 600 }}>{order.orderNumber}</td>
                                    <td>{order.customerName}</td>
                                    <td className="text-muted">{order.vehicleDetails}</td>
                                    <td>
                                        <span className={`badge ${statusBadgeMap[order.status] || 'badge--info'}`}>
                                            {order.status}
                                        </span>
                                    </td>
                                    <td className="text-muted text-sm">
                                        {new Date(order.createdAtUtc).toLocaleDateString('fr-FR')}
                                    </td>
                                </tr>
                            ))
                        )}
                    </tbody>
                </table>
            </div>
        </div>
    );
}
