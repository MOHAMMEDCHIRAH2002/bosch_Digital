import { useState, type FormEvent } from 'react';
import { inventoryApi } from '../api/endpoints';
import { PackagePlus, PackageMinus } from 'lucide-react';

export default function InventoryPage() {
    const [activeTab, setActiveTab] = useState<'receive' | 'issue'>('receive');
    const [message, setMessage] = useState('');

    const [receiveForm, setReceiveForm] = useState({ partNumber: '', description: '', quantity: 1, unitCost: 0, location: '' });
    const [issueForm, setIssueForm] = useState({ partNumber: '', quantity: 1, repairOrderId: '' });

    const handleReceive = async (e: FormEvent) => {
        e.preventDefault();
        try {
            await inventoryApi.receiveStock(receiveForm);
            setMessage('✓ Stock reçu');
        } catch (err: any) { setMessage(`✗ ${err.response?.data?.message || err.message}`); }
    };

    const handleIssue = async (e: FormEvent) => {
        e.preventDefault();
        try {
            await inventoryApi.issuePart(issueForm);
            setMessage('✓ Pièce émise');
        } catch (err: any) { setMessage(`✗ ${err.response?.data?.message || err.message}`); }
    };

    return (
        <div>
            <div className="page-header">
                <h2>Magasin / Inventaire</h2>
                <p>Réception et émission de pièces</p>
            </div>

            {message && <div className="card mt-1" style={{ marginBottom: '1rem', padding: '0.8rem 1rem', fontSize: '0.88rem' }}>{message}</div>}

            <div className="flex gap-1" style={{ marginBottom: '1.5rem' }}>
                <button className={`btn ${activeTab === 'receive' ? 'btn-primary' : 'btn-secondary'}`} onClick={() => setActiveTab('receive')}><PackagePlus size={16} /> Réception</button>
                <button className={`btn ${activeTab === 'issue' ? 'btn-primary' : 'btn-secondary'}`} onClick={() => setActiveTab('issue')}><PackageMinus size={16} /> Émission</button>
            </div>

            <div className="card">
                {activeTab === 'receive' && (
                    <form onSubmit={handleReceive}>
                        <h3 style={{ marginBottom: '1rem' }}>Réception de stock</h3>
                        <div className="form-row">
                            <div className="form-group"><label>N° Pièce</label><input className="form-control" value={receiveForm.partNumber} onChange={(e) => setReceiveForm({ ...receiveForm, partNumber: e.target.value })} required /></div>
                            <div className="form-group"><label>Description</label><input className="form-control" value={receiveForm.description} onChange={(e) => setReceiveForm({ ...receiveForm, description: e.target.value })} required /></div>
                        </div>
                        <div className="form-row">
                            <div className="form-group"><label>Quantité</label><input className="form-control" type="number" value={receiveForm.quantity} onChange={(e) => setReceiveForm({ ...receiveForm, quantity: +e.target.value })} required /></div>
                            <div className="form-group"><label>Coût unitaire (€)</label><input className="form-control" type="number" step="0.01" value={receiveForm.unitCost} onChange={(e) => setReceiveForm({ ...receiveForm, unitCost: +e.target.value })} required /></div>
                        </div>
                        <div className="form-group"><label>Emplacement</label><input className="form-control" value={receiveForm.location} onChange={(e) => setReceiveForm({ ...receiveForm, location: e.target.value })} /></div>
                        <button type="submit" className="btn btn-primary mt-1"><PackagePlus size={16} /> Réceptionner</button>
                    </form>
                )}
                {activeTab === 'issue' && (
                    <form onSubmit={handleIssue}>
                        <h3 style={{ marginBottom: '1rem' }}>Émettre une pièce</h3>
                        <div className="form-row">
                            <div className="form-group"><label>N° Pièce</label><input className="form-control" value={issueForm.partNumber} onChange={(e) => setIssueForm({ ...issueForm, partNumber: e.target.value })} required /></div>
                            <div className="form-group"><label>Quantité</label><input className="form-control" type="number" value={issueForm.quantity} onChange={(e) => setIssueForm({ ...issueForm, quantity: +e.target.value })} required /></div>
                        </div>
                        <div className="form-group"><label>ID Ordre de réparation</label><input className="form-control" value={issueForm.repairOrderId} onChange={(e) => setIssueForm({ ...issueForm, repairOrderId: e.target.value })} required /></div>
                        <button type="submit" className="btn btn-primary mt-1"><PackageMinus size={16} /> Émettre</button>
                    </form>
                )}
            </div>
        </div>
    );
}
