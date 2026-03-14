import { useState, useEffect, type FormEvent } from 'react';
import { customersApi, vehiclesApi, repairOrdersApi } from '../api/endpoints';
import { UserPlus, Car, Plus, Search, Loader2 } from 'lucide-react';

interface Customer {
    id: string;
    firstName: string;
    lastName: string;
    email: string;
    phone: string;
    type: string;
    companyName: string | null;
}

interface Vehicle {
    id: string;
    vin: string;
    licensePlate: string;
    make: string;
    model: string;
    year: number;
    customerId: string;
}

export default function ReceptionPage() {
    const [activeTab, setActiveTab] = useState<'customer' | 'vehicle' | 'or'>('customer');
    const [message, setMessage] = useState('');
    const [isLoading, setIsLoading] = useState(false);

    // Customer form and search
    const [customerForm, setCustomerForm] = useState({ firstName: '', lastName: '', email: '', phone: '', type: 'Individual' });
    const [customerSearch, setCustomerSearch] = useState('');
    const [customers, setCustomers] = useState<Customer[]>([]);
    const [selectedCustomer, setSelectedCustomer] = useState<Customer | null>(null);

    // Vehicle form and search
    const [vehicleForm, setVehicleForm] = useState({ vin: '', licensePlate: '', make: '', model: '', year: 2024, customerId: '' });
    const [vehicleSearch, setVehicleSearch] = useState('');
    const [vehicles, setVehicles] = useState<Vehicle[]>([]);
    const [selectedVehicle, setSelectedVehicle] = useState<Vehicle | null>(null);

    // Repair Order form
    const [orForm, setOrForm] = useState({
        customerId: '',
        vehicleId: '',
        type: 'General',
        visitReason: ''
    });

    // Load customers on mount
    useEffect(() => {
        loadCustomers();
    }, []);

    // Load vehicles when customer is selected
    useEffect(() => {
        if (selectedCustomer) {
            loadVehicles(selectedCustomer.id);
        } else {
            setVehicles([]);
        }
    }, [selectedCustomer]);

    const loadCustomers = async (search?: string) => {
        setIsLoading(true);
        try {
            const res = await customersApi.getAll({ searchTerm: search, page: 1, pageSize: 20 });
            setCustomers(res.data.items || []);
        } catch (err: any) {
            console.error('Failed to load customers:', err);
        } finally {
            setIsLoading(false);
        }
    };

    const loadVehicles = async (customerId?: string, search?: string) => {
        setIsLoading(true);
        try {
            const res = await vehiclesApi.getAll({
                customerId: customerId || undefined,
                searchTerm: search,
                page: 1,
                pageSize: 20
            });
            setVehicles(res.data.items || []);
        } catch (err: any) {
            console.error('Failed to load vehicles:', err);
        } finally {
            setIsLoading(false);
        }
    };

    const handleCustomerSearch = (e: React.ChangeEvent<HTMLInputElement>) => {
        const value = e.target.value;
        setCustomerSearch(value);
        if (value.length >= 2) {
            loadCustomers(value);
        } else if (value.length === 0) {
            loadCustomers();
        }
    };

    const handleVehicleSearch = (e: React.ChangeEvent<HTMLInputElement>) => {
        const value = e.target.value;
        setVehicleSearch(value);
        if (value.length >= 2) {
            loadVehicles(selectedCustomer?.id, value);
        } else if (value.length === 0) {
            loadVehicles(selectedCustomer?.id);
        }
    };

    const handleCustomer = async (e: FormEvent) => {
        e.preventDefault();
        try {
            const res = await customersApi.create(customerForm);
            setMessage(`Client cree — ID: ${res.data}`);
            setCustomerForm({ firstName: '', lastName: '', email: '', phone: '', type: 'Individual' });
            loadCustomers();
        } catch (err: any) {
            setMessage(`Erreur : ${err.response?.data?.message || err.message}`);
        }
    };

    const handleVehicle = async (e: FormEvent) => {
        e.preventDefault();
        try {
            const res = await vehiclesApi.create(vehicleForm);
            setMessage(`Vehicule enregistre — ID: ${res.data}`);
            setVehicleForm({ vin: '', licensePlate: '', make: '', model: '', year: 2024, customerId: '' });
            if (selectedCustomer) {
                loadVehicles(selectedCustomer.id);
            }
        } catch (err: any) {
            setMessage(`Erreur : ${err.response?.data?.message || err.message}`);
        }
    };

    const handleOr = async (e: FormEvent) => {
        e.preventDefault();
        if (!selectedCustomer || !selectedVehicle) {
            setMessage('Veuillez selectionner un client et un vehicule');
            return;
        }
        try {
            const res = await repairOrdersApi.create({
                customerId: selectedCustomer.id,
                vehicleId: selectedVehicle.id,
                type: orForm.type,
                visitReason: orForm.visitReason
            });
            setMessage(`OR cree — ID: ${res.data}`);
            setOrForm({ customerId: '', vehicleId: '', type: 'General', visitReason: '' });
        } catch (err: any) {
            setMessage(`Erreur : ${err.response?.data?.message || err.message}`);
        }
    };

    const selectCustomer = (customer: Customer) => {
        setSelectedCustomer(customer);
        setCustomerSearch(`${customer.firstName} ${customer.lastName} (${customer.email})`);
        setCustomers([]);
    };

    const selectVehicle = (vehicle: Vehicle) => {
        setSelectedVehicle(vehicle);
        setVehicleSearch(`${vehicle.make} ${vehicle.model} (${vehicle.licensePlate})`);
        setVehicles([]);
    };

    const tabs = [
        { key: 'customer' as const, label: 'Nouveau Client', icon: UserPlus },
        { key: 'vehicle' as const, label: 'Nouveau Vehicule', icon: Car },
        { key: 'or' as const, label: 'Ordre de Reparation', icon: Plus },
    ];

    return (
        <div>
            <div className="page-header">
                <h2>Reception</h2>
                <p>Enregistrer un nouveau client, vehicule ou ordre de reparation</p>
            </div>

            {message && (
                <div className="card mt-1" style={{ marginBottom: '1rem', padding: '0.8rem 1rem', fontSize: '0.88rem', backgroundColor: message.includes('Erreur') ? '#fee2e2' : '#dcfce7' }}>
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
                    <div>
                        <h3 style={{ marginBottom: '1rem' }}>Rechercher un client existant</h3>
                        <div className="form-group" style={{ position: 'relative' }}>
                            <label>Recherche client</label>
                            <div style={{ display: 'flex', alignItems: 'center' }}>
                                <Search size={16} style={{ position: 'absolute', left: '12px', color: '#6b7280' }} />
                                <input
                                    className="form-control"
                                    style={{ paddingLeft: '40px' }}
                                    placeholder="Nom, email ou telephone..."
                                    value={customerSearch}
                                    onChange={handleCustomerSearch}
                                />
                            </div>
                            {isLoading && (
                                <div style={{ position: 'absolute', right: '12px', top: '35px' }}>
                                    <Loader2 size={16} className="spin" />
                                </div>
                            )}
                            {customers.length > 0 && (
                                <div style={{
                                    position: 'absolute',
                                    top: '100%',
                                    left: 0,
                                    right: 0,
                                    backgroundColor: 'white',
                                    border: '1px solid #e5e7eb',
                                    borderRadius: '4px',
                                    marginTop: '4px',
                                    maxHeight: '200px',
                                    overflowY: 'auto',
                                    zIndex: 10
                                }}>
                                    {customers.map(c => (
                                        <div
                                            key={c.id}
                                            onClick={() => selectCustomer(c)}
                                            style={{ padding: '10px 12px', cursor: 'pointer', borderBottom: '1px solid #f3f4f6' }}
                                            onMouseEnter={(e) => e.currentTarget.style.backgroundColor = '#f9fafb'}
                                            onMouseLeave={(e) => e.currentTarget.style.backgroundColor = 'white'}
                                        >
                                            <strong>{c.firstName} {c.lastName}</strong>
                                            <div style={{ fontSize: '0.85rem', color: '#6b7280' }}>
                                                {c.email} • {c.phone}
                                            </div>
                                        </div>
                                    ))}
                                </div>
                            )}
                        </div>

                        <hr style={{ margin: '2rem 0', border: 'none', borderTop: '1px solid #e5e7eb' }} />

                        <form onSubmit={handleCustomer}>
                            <h3 style={{ marginBottom: '1rem' }}>Nouveau client</h3>
                            <div className="form-row">
                                <div className="form-group">
                                    <label>Prenom</label>
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
                                    <label>Telephone</label>
                                    <input className="form-control" value={customerForm.phone} onChange={(e) => setCustomerForm({ ...customerForm, phone: e.target.value })} required />
                                </div>
                            </div>
                            <button type="submit" className="btn btn-primary mt-1"><UserPlus size={16} /> Creer le client</button>
                        </form>
                    </div>
                )}

                {activeTab === 'vehicle' && (
                    <div>
                        <h3 style={{ marginBottom: '1rem' }}>Rechercher un vehicule existant</h3>
                        <div className="form-group" style={{ position: 'relative' }}>
                            <label>Recherche vehicule</label>
                            <div style={{ display: 'flex', alignItems: 'center' }}>
                                <Search size={16} style={{ position: 'absolute', left: '12px', color: '#6b7280' }} />
                                <input
                                    className="form-control"
                                    style={{ paddingLeft: '40px' }}
                                    placeholder="Plaque, VIN, marque ou modele..."
                                    value={vehicleSearch}
                                    onChange={handleVehicleSearch}
                                />
                            </div>
                            {isLoading && (
                                <div style={{ position: 'absolute', right: '12px', top: '35px' }}>
                                    <Loader2 size={16} className="spin" />
                                </div>
                            )}
                            {vehicles.length > 0 && (
                                <div style={{
                                    position: 'absolute',
                                    top: '100%',
                                    left: 0,
                                    right: 0,
                                    backgroundColor: 'white',
                                    border: '1px solid #e5e7eb',
                                    borderRadius: '4px',
                                    marginTop: '4px',
                                    maxHeight: '200px',
                                    overflowY: 'auto',
                                    zIndex: 10
                                }}>
                                    {vehicles.map(v => (
                                        <div
                                            key={v.id}
                                            onClick={() => selectVehicle(v)}
                                            style={{ padding: '10px 12px', cursor: 'pointer', borderBottom: '1px solid #f3f4f6' }}
                                            onMouseEnter={(e) => e.currentTarget.style.backgroundColor = '#f9fafb'}
                                            onMouseLeave={(e) => e.currentTarget.style.backgroundColor = 'white'}
                                        >
                                            <strong>{v.make} {v.model}</strong>
                                            <div style={{ fontSize: '0.85rem', color: '#6b7280' }}>
                                                {v.licensePlate} • {v.vin}
                                            </div>
                                        </div>
                                    ))}
                                </div>
                            )}
                        </div>

                        <hr style={{ margin: '2rem 0', border: 'none', borderTop: '1px solid #e5e7eb' }} />

                        <form onSubmit={handleVehicle}>
                            <h3 style={{ marginBottom: '1rem' }}>Enregistrer un nouveau vehicule</h3>
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
                                    <label>Modele</label>
                                    <input className="form-control" value={vehicleForm.model} onChange={(e) => setVehicleForm({ ...vehicleForm, model: e.target.value })} required />
                                </div>
                            </div>
                            <div className="form-row">
                                <div className="form-group">
                                    <label>Annee</label>
                                    <input className="form-control" type="number" value={vehicleForm.year} onChange={(e) => setVehicleForm({ ...vehicleForm, year: +e.target.value })} required />
                                </div>
                                <div className="form-group">
                                    <label>ID Client</label>
                                    <input className="form-control" placeholder="GUID du client" value={vehicleForm.customerId} onChange={(e) => setVehicleForm({ ...vehicleForm, customerId: e.target.value })} required />
                                </div>
                            </div>
                            <button type="submit" className="btn btn-primary mt-1"><Car size={16} /> Enregistrer le vehicule</button>
                        </form>
                    </div>
                )}

                {activeTab === 'or' && (
                    <form onSubmit={handleOr}>
                        <h3 style={{ marginBottom: '1rem' }}>Creer un Ordre de Reparation</h3>

                        <div className="form-group" style={{ position: 'relative', marginBottom: '1.5rem' }}>
                            <label>Selectionner un client</label>
                            <div style={{ display: 'flex', alignItems: 'center' }}>
                                <Search size={16} style={{ position: 'absolute', left: '12px', color: '#6b7280' }} />
                                <input
                                    className="form-control"
                                    style={{ paddingLeft: '40px' }}
                                    placeholder="Rechercher un client..."
                                    value={customerSearch}
                                    onChange={handleCustomerSearch}
                                />
                            </div>
                            {customers.length > 0 && (
                                <div style={{
                                    position: 'absolute',
                                    top: '100%',
                                    left: 0,
                                    right: 0,
                                    backgroundColor: 'white',
                                    border: '1px solid #e5e7eb',
                                    borderRadius: '4px',
                                    marginTop: '4px',
                                    maxHeight: '200px',
                                    overflowY: 'auto',
                                    zIndex: 10
                                }}>
                                    {customers.map(c => (
                                        <div
                                            key={c.id}
                                            onClick={() => selectCustomer(c)}
                                            style={{ padding: '10px 12px', cursor: 'pointer', borderBottom: '1px solid #f3f4f6' }}
                                            onMouseEnter={(e) => e.currentTarget.style.backgroundColor = '#f9fafb'}
                                            onMouseLeave={(e) => e.currentTarget.style.backgroundColor = 'white'}
                                        >
                                            <strong>{c.firstName} {c.lastName}</strong>
                                            <div style={{ fontSize: '0.85rem', color: '#6b7280' }}>
                                                {c.email} • {c.phone}
                                            </div>
                                        </div>
                                    ))}
                                </div>
                            )}
                            {selectedCustomer && (
                                <div style={{ marginTop: '0.5rem', padding: '0.5rem', backgroundColor: '#eff6ff', borderRadius: '4px', fontSize: '0.9rem' }}>
                                    Client selectionne: <strong>{selectedCustomer.firstName} {selectedCustomer.lastName}</strong>
                                </div>
                            )}
                        </div>

                        {selectedCustomer && (
                            <div className="form-group" style={{ position: 'relative', marginBottom: '1.5rem' }}>
                                <label>Selectionner un vehicule</label>
                                <div style={{ display: 'flex', alignItems: 'center' }}>
                                    <Search size={16} style={{ position: 'absolute', left: '12px', color: '#6b7280' }} />
                                    <input
                                        className="form-control"
                                        style={{ paddingLeft: '40px' }}
                                        placeholder="Rechercher un vehicule..."
                                        value={vehicleSearch}
                                        onChange={handleVehicleSearch}
                                    />
                                </div>
                                {vehicles.length > 0 && (
                                    <div style={{
                                        position: 'absolute',
                                        top: '100%',
                                        left: 0,
                                        right: 0,
                                        backgroundColor: 'white',
                                        border: '1px solid #e5e7eb',
                                        borderRadius: '4px',
                                        marginTop: '4px',
                                        maxHeight: '200px',
                                        overflowY: 'auto',
                                        zIndex: 10
                                    }}>
                                        {vehicles.map(v => (
                                            <div
                                                key={v.id}
                                                onClick={() => selectVehicle(v)}
                                                style={{ padding: '10px 12px', cursor: 'pointer', borderBottom: '1px solid #f3f4f6' }}
                                                onMouseEnter={(e) => e.currentTarget.style.backgroundColor = '#f9fafb'}
                                                onMouseLeave={(e) => e.currentTarget.style.backgroundColor = 'white'}
                                            >
                                                <strong>{v.make} {v.model}</strong>
                                                <div style={{ fontSize: '0.85rem', color: '#6b7280' }}>
                                                    {v.licensePlate} • {v.vin}
                                                </div>
                                            </div>
                                        ))}
                                    </div>
                                )}
                                {selectedVehicle && (
                                    <div style={{ marginTop: '0.5rem', padding: '0.5rem', backgroundColor: '#eff6ff', borderRadius: '4px', fontSize: '0.9rem' }}>
                                        Vehicule selectionne: <strong>{selectedVehicle.make} {selectedVehicle.model}</strong> ({selectedVehicle.licensePlate})
                                    </div>
                                )}
                            </div>
                        )}

                        <div className="form-row">
                            <div className="form-group">
                                <label>Type d'ordre</label>
                                <select className="form-control" value={orForm.type} onChange={(e) => setOrForm({ ...orForm, type: e.target.value })}>
                                    <option value="General">General</option>
                                    <option value="ServiceRapide">Service Rapide</option>
                                    <option value="RetourTechnique">Retour Technique</option>
                                    <option value="Sinistre">Sinistre</option>
                                </select>
                            </div>
                        </div>

                        <div className="form-group">
                            <label>Motif de visite / Symptomes</label>
                            <textarea className="form-control" rows={3} value={orForm.visitReason} onChange={(e) => setOrForm({ ...orForm, visitReason: e.target.value })} placeholder="Description des symptomes ou raison de la visite..." />
                        </div>

                        <button type="submit" className="btn btn-primary mt-1" disabled={!selectedCustomer || !selectedVehicle}>
                            <Plus size={16} /> Creer l'OR
                        </button>
                    </form>
                )}
            </div>
        </div>
    );
}
