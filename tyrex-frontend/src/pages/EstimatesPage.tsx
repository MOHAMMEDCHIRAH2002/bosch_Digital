import { useState, type FormEvent } from 'react';
import { estimatesApi } from '../api/endpoints';
import { FileText, Check, X } from 'lucide-react';

export default function EstimatesPage() {
    const [activeTab, setActiveTab] = useState<'generate' | 'approve' | 'refuse'>('generate');
    const [message, setMessage] = useState('');

    const [genForm, setGenForm] = useState({ repairOrderId: '', description: '', quantity: 1, unitPrice: 0, taxRate: 20 });
    const [approveForm, setApproveForm] = useState({ estimateId: '', proofUrl: '' });
    const [refuseId, setRefuseId] = useState('');

    const handleGenerate = async (e: FormEvent) => {
        e.preventDefault();
        try {
            const res = await estimatesApi.generate({
                repairOrderId: genForm.repairOrderId,
                items: [{ description: genForm.description, quantity: genForm.quantity, unitPrice: genForm.unitPrice, taxRate: genForm.taxRate }],
            });
            setMessage(`✓ Devis créé — ID: ${res.data}`);
        } catch (err: any) { setMessage(`✗ ${err.response?.data?.message || err.message}`); }
    };

    const handleApprove = async (e: FormEvent) => {
        e.preventDefault();
        try {
            await estimatesApi.approve(approveForm.estimateId, { clientApprovalProofUrl: approveForm.proofUrl });
            setMessage('✓ Devis approuvé');
        } catch (err: any) { setMessage(`✗ ${err.response?.data?.message || err.message}`); }
    };

    const handleRefuse = async (e: FormEvent) => {
        e.preventDefault();
        try {
            await estimatesApi.refuse(refuseId);
            setMessage('✓ Devis refusé');
        } catch (err: any) { setMessage(`✗ ${err.response?.data?.message || err.message}`); }
    };

    return (
        <div>
            <div className="page-header">
                <h2>Devis</h2>
                <p>Créer, approuver et gérer les devis</p>
            </div>

            {message && <div className="card mt-1" style={{ marginBottom: '1rem', padding: '0.8rem 1rem', fontSize: '0.88rem' }}>{message}</div>}

            <div className="flex gap-1" style={{ marginBottom: '1.5rem' }}>
                <button className={`btn ${activeTab === 'generate' ? 'btn-primary' : 'btn-secondary'}`} onClick={() => setActiveTab('generate')}><FileText size={16} /> Générer</button>
                <button className={`btn ${activeTab === 'approve' ? 'btn-primary' : 'btn-secondary'}`} onClick={() => setActiveTab('approve')}><Check size={16} /> Approuver</button>
                <button className={`btn ${activeTab === 'refuse' ? 'btn-primary' : 'btn-secondary'}`} onClick={() => setActiveTab('refuse')}><X size={16} /> Refuser</button>
            </div>

            <div className="card">
                {activeTab === 'generate' && (
                    <form onSubmit={handleGenerate}>
                        <h3 style={{ marginBottom: '1rem' }}>Générer un devis</h3>
                        <div className="form-group"><label>ID Ordre de réparation</label><input className="form-control" value={genForm.repairOrderId} onChange={(e) => setGenForm({ ...genForm, repairOrderId: e.target.value })} required /></div>
                        <div className="form-group"><label>Description ligne</label><input className="form-control" value={genForm.description} onChange={(e) => setGenForm({ ...genForm, description: e.target.value })} required /></div>
                        <div className="form-row">
                            <div className="form-group"><label>Quantité</label><input className="form-control" type="number" value={genForm.quantity} onChange={(e) => setGenForm({ ...genForm, quantity: +e.target.value })} required /></div>
                            <div className="form-group"><label>Prix unitaire (€)</label><input className="form-control" type="number" step="0.01" value={genForm.unitPrice} onChange={(e) => setGenForm({ ...genForm, unitPrice: +e.target.value })} required /></div>
                        </div>
                        <button type="submit" className="btn btn-primary mt-1"><FileText size={16} /> Générer</button>
                    </form>
                )}
                {activeTab === 'approve' && (
                    <form onSubmit={handleApprove}>
                        <h3 style={{ marginBottom: '1rem' }}>Approuver un devis</h3>
                        <div className="form-group"><label>ID Devis</label><input className="form-control" value={approveForm.estimateId} onChange={(e) => setApproveForm({ ...approveForm, estimateId: e.target.value })} required /></div>
                        <div className="form-group"><label>URL Preuve d'approbation</label><input className="form-control" value={approveForm.proofUrl} onChange={(e) => setApproveForm({ ...approveForm, proofUrl: e.target.value })} required /></div>
                        <button type="submit" className="btn btn-primary mt-1"><Check size={16} /> Approuver</button>
                    </form>
                )}
                {activeTab === 'refuse' && (
                    <form onSubmit={handleRefuse}>
                        <h3 style={{ marginBottom: '1rem' }}>Refuser un devis</h3>
                        <div className="form-group"><label>ID Devis</label><input className="form-control" value={refuseId} onChange={(e) => setRefuseId(e.target.value)} required /></div>
                        <button type="submit" className="btn btn-danger mt-1"><X size={16} /> Refuser</button>
                    </form>
                )}
            </div>
        </div>
    );
}
