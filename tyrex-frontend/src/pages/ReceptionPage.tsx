import { useState, useEffect, type FormEvent, useRef, type ChangeEvent } from 'react';
import { customersApi, vehiclesApi, repairOrdersApi, filesApi } from '../api/endpoints';
import { UserPlus, Car, Plus, Search, Loader2, Camera, FileImage, X, Check, Building2 } from 'lucide-react';

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

interface CreatedOR {
    id: string;
    orderNumber: string;
    customerName: string;
    vehicleInfo: string;
}

export default function ReceptionPage() {
    const [activeTab, setActiveTab] = useState<'customer' | 'vehicle' | 'or'>('customer');
    const [message, setMessage] = useState('');
    const [isLoading, setIsLoading] = useState(false);
    const [createdOR, setCreatedOR] = useState<CreatedOR | null>(null);

    // Customer form and search
    const [customerForm, setCustomerForm] = useState({
        firstName: '',
        lastName: '',
        email: '',
        phone: '',
        type: 'Individual',
        companyName: ''
    });
    const [customerSearch, setCustomerSearch] = useState('');
    const [customers, setCustomers] = useState<Customer[]>([]);
    const [selectedCustomer, setSelectedCustomer] = useState<Customer | null>(null);

    // Vehicle form and search
    const [vehicleForm, setVehicleForm] = useState({ vin: '', licensePlate: '', make: '', model: '', year: 2024, customerId: '', isInternalFleet: false });
    const [vehicleSearch, setVehicleSearch] = useState('');
    const [vehicles, setVehicles] = useState<Vehicle[]>([]);
    const [selectedVehicle, setSelectedVehicle] = useState<Vehicle | null>(null);

    // Repair Order form
    const [orForm, setOrForm] = useState({
        customerId: '',
        vehicleId: '',
        type: 'General',
        visitReason: '',
        intakeMileage: ''
    });

    // Photo upload
    const [selectedPhotos, setSelectedPhotos] = useState<File[]>([]);
    const [uploadingPhotos, setUploadingPhotos] = useState(false);
    const fileInputRef = useRef<HTMLInputElement>(null);

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
            const payload = {
                ...customerForm,
                companyName: customerForm.type === 'Company' ? customerForm.companyName : null
            };
            const res = await customersApi.create(payload);
            setMessage(`Client créé avec succès!`);
            setCustomerForm({ firstName: '', lastName: '', email: '', phone: '', type: 'Individual', companyName: '' });
            loadCustomers();
        } catch (err: any) {
            setMessage(`Erreur : ${err.response?.data?.message || err.message}`);
        }
    };

    const handleVehicle = async (e: FormEvent) => {
        e.preventDefault();
        if (!vehicleForm.customerId) {
            setMessage('Erreur: Veuillez sélectionner un client pour ce véhicule');
            return;
        }
        try {
            const res = await vehiclesApi.create(vehicleForm);
            setMessage(`Véhicule enregistré avec succès!`);
            setVehicleForm({ vin: '', licensePlate: '', make: '', model: '', year: 2024, customerId: '', isInternalFleet: false });
            if (selectedCustomer) {
                loadVehicles(selectedCustomer.id);
            }
        } catch (err: any) {
            setMessage(`Erreur : ${err.response?.data?.message || err.message}`);
        }
    };

    const handlePhotoSelect = (e: ChangeEvent<HTMLInputElement>) => {
        if (e.target.files) {
            const files = Array.from(e.target.files);
            setSelectedPhotos(prev => [...prev, ...files]);
        }
    };

    const removePhoto = (index: number) => {
        setSelectedPhotos(prev => prev.filter((_, i) => i !== index));
    };

    const uploadPhotos = async (repairOrderId: string): Promise<string[]> => {
        if (selectedPhotos.length === 0) return [];

        setUploadingPhotos(true);
        const uploadedUrls: string[] = [];

        try {
            for (const photo of selectedPhotos) {
                const res = await filesApi.upload(photo, 'intake-photos');
                uploadedUrls.push(res.data.url);
            }

            // Attach photos to repair order
            if (uploadedUrls.length > 0) {
                await repairOrdersApi.addPhotos(repairOrderId, uploadedUrls);
            }
        } catch (err: any) {
            console.error('Failed to upload photos:', err);
        } finally {
            setUploadingPhotos(false);
        }

        return uploadedUrls;
    };

    const handleOr = async (e: FormEvent) => {
        e.preventDefault();
        if (!selectedCustomer || !selectedVehicle) {
            setMessage('Veuillez sélectionner un client et un véhicule');
            return;
        }
        try {
            const res = await repairOrdersApi.create({
                customerId: selectedCustomer.id,
                vehicleId: selectedVehicle.id,
                type: orForm.type,
                visitReason: orForm.visitReason,
                intakeMileage: orForm.intakeMileage ? parseInt(orForm.intakeMileage) : null
            });

            const repairOrderId = res.data;

            // Upload photos if any
            if (selectedPhotos.length > 0) {
                await uploadPhotos(repairOrderId);
            }

            setCreatedOR({
                id: repairOrderId,
                orderNumber: 'OR-' + repairOrderId.slice(0, 8).toUpperCase(),
                customerName: `${selectedCustomer.firstName} ${selectedCustomer.lastName}`,
                vehicleInfo: `${selectedVehicle.make} ${selectedVehicle.model} (${selectedVehicle.licensePlate})`
            });

            setMessage(`Ordre de Réparation créé avec succès!`);
            setOrForm({ customerId: '', vehicleId: '', type: 'General', visitReason: '', intakeMileage: '' });
            setSelectedPhotos([]);
        } catch (err: any) {
            setMessage(`Erreur : ${err.response?.data?.message || err.message}`);
        }
    };

    const selectCustomer = (customer: Customer) => {
        setSelectedCustomer(customer);
        setCustomerSearch(`${customer.firstName} ${customer.lastName} (${customer.email})`);
        setCustomers([]);
        // Pre-fill customer ID for vehicle creation
        setVehicleForm(prev => ({ ...prev, customerId: customer.id }));
    };

    const selectVehicle = (vehicle: Vehicle) => {
        setSelectedVehicle(vehicle);
        setVehicleSearch(`${vehicle.make} ${vehicle.model} (${vehicle.licensePlate})`);
        setVehicles([]);
    };

    const clearCreatedOR = () => {
        setCreatedOR(null);
        setMessage('');
    };

    const tabs = [
        { key: 'customer' as const, label: 'Nouveau Client', icon: UserPlus },
        { key: 'vehicle' as const, label: 'Nouveau Véhicule', icon: Car },
        { key: 'or' as const, label: 'Ordre de Réparation', icon: Plus },
    ];

    return (
        <div>
            <div className="page-header">
                <h2>Réception</h2>
                <p>Enregistrer un nouveau client, véhicule ou ordre de réparation</p>
            </div>

            {message && (
                <div className={`card mt-1 ${message.includes('Erreur') ? 'error' : 'success'}`}
                     style={{ marginBottom: '1rem', padding: '0.8rem 1rem', fontSize: '0.88rem',
                              backgroundColor: message.includes('Erreur') ? '#fee2e2' : '#dcfce7',
                              borderLeft: `4px solid ${message.includes('Erreur') ? '#ef4444' : '#22c55e'}` }}>
                    {message}
                </div>
            )}

            {/* Success Card for Created OR */}
            {createdOR && (
                <div className="card" style={{ marginBottom: '1.5rem', padding: '1.5rem', backgroundColor: '#f0fdf4', border: '2px solid #22c55e' }}>
                    <div style={{ display: 'flex', alignItems: 'center', gap: '1rem', marginBottom: '1rem' }}>
                        <div style={{ backgroundColor: '#22c55e', color: 'white', borderRadius: '50%', padding: '0.5rem' }}>
                            <Check size={24} />
                        </div>
                        <div>
                            <h3 style={{ margin: 0, color: '#166534' }}>Ordre de Réparation Créé!</h3>
                            <p style={{ margin: 0, color: '#166534', fontSize: '1.5rem', fontWeight: 'bold' }}>{createdOR.orderNumber}</p>
                        </div>
                    </div>
                    <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '1rem', marginBottom: '1rem' }}>
                        <div>
                            <label style={{ fontSize: '0.85rem', color: '#6b7280' }}>Client</label>
                            <p style={{ margin: 0, fontWeight: 500 }}>{createdOR.customerName}</p>
                        </div>
                        <div>
                            <label style={{ fontSize: '0.85rem', color: '#6b7280' }}>Véhicule</label>
                            <p style={{ margin: 0, fontWeight: 500 }}>{createdOR.vehicleInfo}</p>
                        </div>
                    </div>
                    <button className="btn btn-primary" onClick={clearCreatedOR}>
                        Créer un nouvel OR
                    </button>
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
                                    placeholder="Nom, email ou téléphone..."
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
                                            {c.companyName && <span style={{ marginLeft: '8px', color: '#6b7280' }}>({c.companyName})</span>}
                                            <div style={{ fontSize: '0.85rem', color: '#6b7280' }}>
                                                {c.email} • {c.phone} • {c.type === 'Company' ? 'Entreprise' : 'Particulier'}
                                            </div>
                                        </div>
                                    ))}
                                </div>
                            )}
                        </div>

                        <hr style={{ margin: '2rem 0', border: 'none', borderTop: '1px solid #e5e7eb' }} />

                        <form onSubmit={handleCustomer}>
                            <h3 style={{ marginBottom: '1rem' }}>Nouveau client</h3>

                            {/* Customer Type Selection */}
                            <div className="form-group">
                                <label>Type de client</label>
                                <div style={{ display: 'flex', gap: '1rem' }}>
                                    <label style={{ display: 'flex', alignItems: 'center', gap: '0.5rem', cursor: 'pointer', padding: '0.5rem 1rem', border: `2px solid ${customerForm.type === 'Individual' ? '#2563eb' : '#e5e7eb'}`, borderRadius: '4px' }}>
                                        <input
                                            type="radio"
                                            name="customerType"
                                            value="Individual"
                                            checked={customerForm.type === 'Individual'}
                                            onChange={(e) => setCustomerForm({ ...customerForm, type: e.target.value })}
                                        />
                                        <UserPlus size={16} />
                                        Particulier
                                    </label>
                                    <label style={{ display: 'flex', alignItems: 'center', gap: '0.5rem', cursor: 'pointer', padding: '0.5rem 1rem', border: `2px solid ${customerForm.type === 'Company' ? '#2563eb' : '#e5e7eb'}`, borderRadius: '4px' }}>
                                        <input
                                            type="radio"
                                            name="customerType"
                                            value="Company"
                                            checked={customerForm.type === 'Company'}
                                            onChange={(e) => setCustomerForm({ ...customerForm, type: e.target.value })}
                                        />
                                        <Building2 size={16} />
                                        Entreprise
                                    </label>
                                </div>
                            </div>

                            {customerForm.type === 'Company' && (
                                <div className="form-group">
                                    <label>Nom de l'entreprise</label>
                                    <input
                                        className="form-control"
                                        value={customerForm.companyName}
                                        onChange={(e) => setCustomerForm({ ...customerForm, companyName: e.target.value })}
                                        required={customerForm.type === 'Company'}
                                    />
                                </div>
                            )}

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
                    </div>
                )}

                {activeTab === 'vehicle' && (
                    <div>
                        <h3 style={{ marginBottom: '1rem' }}>Rechercher un véhicule existant</h3>
                        <div className="form-group" style={{ position: 'relative' }}>
                            <label>Recherche véhicule</label>
                            <div style={{ display: 'flex', alignItems: 'center' }}>
                                <Search size={16} style={{ position: 'absolute', left: '12px', color: '#6b7280' }} />
                                <input
                                    className="form-control"
                                    style={{ paddingLeft: '40px' }}
                                    placeholder="Plaque, VIN, marque ou modèle..."
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
                            <h3 style={{ marginBottom: '1rem' }}>Enregistrer un nouveau véhicule</h3>

                            {/* Customer Selection for Vehicle */}
                            <div className="form-group" style={{ position: 'relative' }}>
                                <label>Client propriétaire <span style={{ color: '#ef4444' }}>*</span></label>
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
                                                onClick={() => {
                                                    selectCustomer(c);
                                                    setVehicleForm(prev => ({ ...prev, customerId: c.id }));
                                                }}
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
                                {vehicleForm.customerId && selectedCustomer && (
                                    <div style={{ marginTop: '0.5rem', padding: '0.5rem', backgroundColor: '#eff6ff', borderRadius: '4px', fontSize: '0.9rem' }}>
                                        Client sélectionné: <strong>{selectedCustomer.firstName} {selectedCustomer.lastName}</strong>
                                    </div>
                                )}
                            </div>

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
                                <div className="form-group" style={{ display: 'flex', alignItems: 'flex-end' }}>
                                    <label style={{ display: 'flex', alignItems: 'center', gap: '0.5rem', cursor: 'pointer', marginBottom: '0.5rem' }}>
                                        <input
                                            type="checkbox"
                                            checked={vehicleForm.isInternalFleet}
                                            onChange={(e) => setVehicleForm({ ...vehicleForm, isInternalFleet: e.target.checked })}
                                        />
                                        Véhicule interne (flotte)
                                    </label>
                                </div>
                            </div>
                            <button type="submit" className="btn btn-primary mt-1" disabled={!vehicleForm.customerId}><Car size={16} /> Enregistrer le véhicule</button>
                        </form>
                    </div>
                )}

                {activeTab === 'or' && (
                    <form onSubmit={handleOr}>
                        <h3 style={{ marginBottom: '1rem' }}>Créer un Ordre de Réparation</h3>

                        <div className="form-group" style={{ position: 'relative', marginBottom: '1.5rem' }}>
                            <label>Sélectionner un client <span style={{ color: '#ef4444' }}>*</span></label>
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
                                            {c.companyName && <span style={{ marginLeft: '8px', color: '#6b7280' }}>({c.companyName})</span>}
                                            <div style={{ fontSize: '0.85rem', color: '#6b7280' }}>
                                                {c.email} • {c.phone}
                                            </div>
                                        </div>
                                    ))}
                                </div>
                            )}
                            {selectedCustomer && (
                                <div style={{ marginTop: '0.5rem', padding: '0.75rem', backgroundColor: '#eff6ff', borderRadius: '4px', fontSize: '0.9rem', border: '1px solid #2563eb' }}>
                                    <strong>Client sélectionné:</strong> {selectedCustomer.firstName} {selectedCustomer.lastName}
                                    <div style={{ fontSize: '0.85rem', color: '#6b7280' }}>{selectedCustomer.email} • {selectedCustomer.phone}</div>
                                </div>
                            )}
                        </div>

                        {selectedCustomer && (
                            <div className="form-group" style={{ position: 'relative', marginBottom: '1.5rem' }}>
                                <label>Sélectionner un véhicule <span style={{ color: '#ef4444' }}>*</span></label>
                                <div style={{ display: 'flex', alignItems: 'center' }}>
                                    <Search size={16} style={{ position: 'absolute', left: '12px', color: '#6b7280' }} />
                                    <input
                                        className="form-control"
                                        style={{ paddingLeft: '40px' }}
                                        placeholder="Rechercher un véhicule..."
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
                                    <div style={{ marginTop: '0.5rem', padding: '0.75rem', backgroundColor: '#eff6ff', borderRadius: '4px', fontSize: '0.9rem', border: '1px solid #2563eb' }}>
                                        <strong>Véhicule sélectionné:</strong> {selectedVehicle.make} {selectedVehicle.model}
                                        <div style={{ fontSize: '0.85rem', color: '#6b7280' }}>{selectedVehicle.licensePlate} • {selectedVehicle.vin}</div>
                                    </div>
                                )}
                            </div>
                        )}

                        <div className="form-row">
                            <div className="form-group">
                                <label>Type d'ordre</label>
                                <select className="form-control" value={orForm.type} onChange={(e) => setOrForm({ ...orForm, type: e.target.value })}>
                                    <option value="General">Général</option>
                                    <option value="ServiceRapide">Service Rapide</option>
                                    <option value="RetourTechnique">Retour Technique</option>
                                    <option value="Sinistre">Sinistre</option>
                                </select>
                            </div>
                            <div className="form-group">
                                <label>Kilométrage au dépôt</label>
                                <input
                                    className="form-control"
                                    type="number"
                                    placeholder="Ex: 50000"
                                    value={orForm.intakeMileage}
                                    onChange={(e) => setOrForm({ ...orForm, intakeMileage: e.target.value })}
                                />
                            </div>
                        </div>

                        <div className="form-group">
                            <label>Motif de visite / Symptômes <span style={{ color: '#ef4444' }}>*</span></label>
                            <textarea
                                className="form-control"
                                rows={3}
                                value={orForm.visitReason}
                                onChange={(e) => setOrForm({ ...orForm, visitReason: e.target.value })}
                                placeholder="Description des symptômes ou raison de la visite..."
                                required
                            />
                        </div>

                        {/* Photo Upload */}
                        <div className="form-group">
                            <label>Photos d'état à l'arrivée</label>
                            <input
                                type="file"
                                ref={fileInputRef}
                                style={{ display: 'none' }}
                                accept="image/*"
                                multiple
                                onChange={handlePhotoSelect}
                            />
                            <button
                                type="button"
                                className="btn btn-secondary"
                                onClick={() => fileInputRef.current?.click()}
                                style={{ display: 'flex', alignItems: 'center', gap: '0.5rem' }}
                            >
                                <Camera size={16} />
                                Ajouter des photos
                            </button>

                            {selectedPhotos.length > 0 && (
                                <div style={{ display: 'flex', gap: '0.5rem', flexWrap: 'wrap', marginTop: '0.75rem' }}>
                                    {selectedPhotos.map((photo, index) => (
                                        <div key={index} style={{ position: 'relative', width: '80px', height: '80px', borderRadius: '4px', overflow: 'hidden', border: '1px solid #e5e7eb' }}>
                                            <img
                                                src={URL.createObjectURL(photo)}
                                                alt={`Photo ${index + 1}`}
                                                style={{ width: '100%', height: '100%', objectFit: 'cover' }}
                                            />
                                            <button
                                                type="button"
                                                onClick={() => removePhoto(index)}
                                                style={{
                                                    position: 'absolute',
                                                    top: '2px',
                                                    right: '2px',
                                                    backgroundColor: '#ef4444',
                                                    color: 'white',
                                                    border: 'none',
                                                    borderRadius: '50%',
                                                    width: '20px',
                                                    height: '20px',
                                                    display: 'flex',
                                                    alignItems: 'center',
                                                    justifyContent: 'center',
                                                    cursor: 'pointer',
                                                    fontSize: '12px'
                                                }}
                                            >
                                                <X size={12} />
                                            </button>
                                        </div>
                                    ))}
                                </div>
                            )}
                        </div>

                        <button type="submit" className="btn btn-primary mt-1" disabled={!selectedCustomer || !selectedVehicle || uploadingPhotos}>
                            <Plus size={16} />
                            {uploadingPhotos ? 'Traitement en cours...' : 'Créer l\'OR'}
                        </button>
                    </form>
                )}
            </div>
        </div>
    );
}
