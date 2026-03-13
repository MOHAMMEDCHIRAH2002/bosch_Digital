import { useState, type FormEvent } from 'react';
import { customersApi, vehiclesApi, repairOrdersApi } from '../api/endpoints';
import { UserPlus, Car, Plus } from 'lucide-react';

export default function ReceptionPage() {
    const [activeTab, setActiveTab] = useState<'customer' | 'vehicle' | 'or'>('customer');
    const [message, setMessage] = useState('');

    // Customer form
    const [customerForm, setCustomerForm] = useState({ firstName: '', lastName: '', email: '', phone: '', type: 'Individual' });
    // Vehicle form
    const [vehicleForm, setVehicleForm] = useState({ vin: '', licensePlate: '', make: '', model: '', year: 2024, customerId: '' });
    // Repair Order form
    const [orForm, setOrForm] = useState({ customerId: '', vehicleId: '', type: 'Standard', description: '' });

    const handleCustomer = async (e: FormEvent) => {
        e.preventDefault();
        try {
            const res = await customersApi.create(customerForm);
            setMessage(`✓ Client créé — ID: ${res.data}`);
        } catch (err: any) {
            setMessage(`✗ Erreur : ${err.response?.data?.message || err.message}`);
        }
    };

    const handleVehicle = async (e: FormEvent) => {
        e.preventDefault();
        try {
            const res = await vehiclesApi.create(vehicleForm);
            setMessage(`✓ Véhicule enregistré — ID: ${res.data}`);
        } catch (err: any) {
            setMessage(`✗ Erreur : ${err.response?.data?.message || err.message}`);
        }
    };

    const handleOr = async (e: FormEvent) => {
        e.preventDefault();
        try {
            const res = await repairOrdersApi.create(orForm);
            setMessage(`✓ OR créé — ID: ${res.data}`);
        } catch (err: any) {
            setMessage(`✗ Erreur : ${err.response?.data?.message || err.message}`);
        }
    };

    const tabs = [
        { key: 'customer' as const, label: 'Client', icon: UserPlus },
        { key: 'vehicle' as const, label: 'Véhicule', icon: Car },
        { key: 'or' as const, label: 'Ordre de Réparation', icon: Plus },
    ];

    return (
        <div>
            <div className="page-header">
                <h2>Réception</h2>
                <p>Enregistrer un nouveau client, véhicule ou ordre de réparation</p>
            </div>

            {message && (
                <div className={`card mt-1`} style={{ marginBottom: '1rem', padding: '0.8rem 1rem', fontSize: '0.88rem' }}>
                    {message}
                </div>
            )}

            <div className="flex gap-1" style={{ marginBottom: '1.5rem' }}>
                {tabs.map((tab) => (
                    <button
                        key={tab.key}
                        className={`btn ${activeTab === tab.key ? 'btn-primary' : 'btn-secondary'}`}
                        onClick={() => setActiveTab(tab.key)}
                    >
                        <tab.icon size={16} /> {tab.label}
                    </button>
                ))}
            </div>

            <div className="card">
                {activeTab === 'customer' && (
                    <form onSubmit={handleCustomer}>
                        <h3 style={{ marginBottom: '1rem' }}>Nouveau client</h3>
                        <div className="form-row">
                            <div className="form-group">
                                <label>Prénom</label>
                                <input className="form-control" value={customerForm.firstName} onChange={(e) => setCustomerForm({ ...customerForm, firstName: e.target.value })} required />
                            </div>
                            <div className="form-group">
                                <label>Nom</label>
                                <input className="form-control" value={customerForm.lastName} onChange={(e) => setCustomerForm({ ...customerForm, lastName: e.target.value })} required />
                            </div>
                        </div>
                        <div className="form-row">
                            <div className="form-group">
                                <label>Email</label>
                                <input className="form-control" type="email" value={customerForm.email} onChange={(e) => setCustomerForm({ ...customerForm, email: e.target.value })} required />
                            </div>
                            <div className="form-group">
                                <label>Téléphone</label>
                                <input className="form-control" value={customerForm.phone} onChange={(e) => setCustomerForm({ ...customerForm, phone: e.target.value })} required />
                            </div>
                        </div>
                        <button type="submit" className="btn btn-primary mt-1"><UserPlus size={16} /> Créer le client</button>
                    </form>
                )}

                {activeTab === 'vehicle' && (
                    <form onSubmit={handleVehicle}>
                        <h3 style={{ marginBottom: '1rem' }}>Enregistrer un véhicule</h3>
                        <div className="form-row">
                            <div className="form-group">
                                <label>VIN</label>
                                <input className="form-control" value={vehicleForm.vin} onChange={(e) => setVehicleForm({ ...vehicleForm, vin: e.target.value })} required />
                            </div>
                            <div className="form-group">
                                <label>Plaque d'immatriculation</label>
                                <input className="form-control" value={vehicleForm.licensePlate} onChange={(e) => setVehicleForm({ ...vehicleForm, licensePlate: e.target.value })} required />
                            </div>
                        </div>
                        <div className="form-row">
                            <div className="form-group">
                                <label>Marque</label>
                                <input className="form-control" value={vehicleForm.make} onChange={(e) => setVehicleForm({ ...vehicleForm, make: e.target.value })} required />
                            </div>
                            <div className="form-group">
                                <label>Modèle</label>
                                <input className="form-control" value={vehicleForm.model} onChange={(e) => setVehicleForm({ ...vehicleForm, model: e.target.value })} required />
                            </div>
                        </div>
                        <div className="form-row">
                            <div className="form-group">
                                <label>Année</label>
                                <input className="form-control" type="number" value={vehicleForm.year} onChange={(e) => setVehicleForm({ ...vehicleForm, year: +e.target.value })} required />
                            </div>
                            <div className="form-group">
                                <label>ID Client</label>
                                <input className="form-control" placeholder="GUID du client" value={vehicleForm.customerId} onChange={(e) => setVehicleForm({ ...vehicleForm, customerId: e.target.value })} required />
                            </div>
                        </div>
                        <button type="submit" className="btn btn-primary mt-1"><Car size={16} /> Enregistrer le véhicule</button>
                    </form>
                )}

                {activeTab === 'or' && (
                    <form onSubmit={handleOr}>
                        <h3 style={{ marginBottom: '1rem' }}>Créer un Ordre de Réparation</h3>
                        <div className="form-row">
                            <div className="form-group">
                                <label>ID Client</label>
                                <input className="form-control" placeholder="GUID du client" value={orForm.customerId} onChange={(e) => setOrForm({ ...orForm, customerId: e.target.value })} required />
                            </div>
                            <div className="form-group">
                                <label>ID Véhicule</label>
                                <input className="form-control" placeholder="GUID du véhicule" value={orForm.vehicleId} onChange={(e) => setOrForm({ ...orForm, vehicleId: e.target.value })} required />
                            </div>
                        </div>
                        <div className="form-group">
                            <label>Description</label>
                            <textarea className="form-control" rows={3} value={orForm.description} onChange={(e) => setOrForm({ ...orForm, description: e.target.value })} />
                        </div>
                        <button type="submit" className="btn btn-primary mt-1"><Plus size={16} /> Créer l'OR</button>
                    </form>
                )}
            </div>
        </div>
    );
}
