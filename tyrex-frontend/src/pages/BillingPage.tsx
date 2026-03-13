import { useState, type FormEvent } from 'react';
import { billingApi } from '../api/endpoints';
import { Receipt, DollarSign } from 'lucide-react';

export default function BillingPage() {
    const [activeTab, setActiveTab] = useState<'invoice' | 'payment'>('invoice');
    const [message, setMessage] = useState('');

    const [invoiceForm, setInvoiceForm] = useState({ repairOrderId: '', dueDate: '' });
    const [payForm, setPayForm] = useState({ invoiceId: '', amount: 0, method: 'Card', referenceInfo: '' });

    const handleInvoice = async (e: FormEvent) => {
        e.preventDefault();
        try {
            const res = await billingApi.generateInvoice({ repairOrderId: invoiceForm.repairOrderId, dueDate: invoiceForm.dueDate });
            setMessage(`✓ Facture créée — ID: ${res.data}`);
        } catch (err: any) { setMessage(`✗ ${err.response?.data?.message || err.message}`); }
    };

    const handlePayment = async (e: FormEvent) => {
        e.preventDefault();
        try {
            await billingApi.registerPayment(payForm.invoiceId, { amount: payForm.amount, method: payForm.method === 'Card' ? 1 : payForm.method === 'Cash' ? 0 : 2, referenceInfo: payForm.referenceInfo });
            setMessage('✓ Paiement enregistré');
        } catch (err: any) { setMessage(`✗ ${err.response?.data?.message || err.message}`); }
    };

    return (
        <div>
            <div className="page-header">
                <h2>Facturation</h2>
                <p>Créer des factures et enregistrer des paiements</p>
            </div>

            {message && <div className="card mt-1" style={{ marginBottom: '1rem', padding: '0.8rem 1rem', fontSize: '0.88rem' }}>{message}</div>}

            <div className="flex gap-1" style={{ marginBottom: '1.5rem' }}>
                <button className={`btn ${activeTab === 'invoice' ? 'btn-primary' : 'btn-secondary'}`} onClick={() => setActiveTab('invoice')}><Receipt size={16} /> Facturer</button>
                <button className={`btn ${activeTab === 'payment' ? 'btn-primary' : 'btn-secondary'}`} onClick={() => setActiveTab('payment')}><DollarSign size={16} /> Paiement</button>
            </div>

            <div className="card">
                {activeTab === 'invoice' && (
                    <form onSubmit={handleInvoice}>
                        <h3 style={{ marginBottom: '1rem' }}>Générer une facture</h3>
                        <div className="form-row">
                            <div className="form-group"><label>ID Ordre de réparation</label><input className="form-control" value={invoiceForm.repairOrderId} onChange={(e) => setInvoiceForm({ ...invoiceForm, repairOrderId: e.target.value })} required /></div>
                            <div className="form-group"><label>Date d'échéance</label><input className="form-control" type="date" value={invoiceForm.dueDate} onChange={(e) => setInvoiceForm({ ...invoiceForm, dueDate: e.target.value })} required /></div>
                        </div>
                        <button type="submit" className="btn btn-primary mt-1"><Receipt size={16} /> Générer la facture</button>
                    </form>
                )}
                {activeTab === 'payment' && (
                    <form onSubmit={handlePayment}>
                        <h3 style={{ marginBottom: '1rem' }}>Enregistrer un paiement</h3>
                        <div className="form-group"><label>ID Facture</label><input className="form-control" value={payForm.invoiceId} onChange={(e) => setPayForm({ ...payForm, invoiceId: e.target.value })} required /></div>
                        <div className="form-row">
                            <div className="form-group"><label>Montant (€)</label><input className="form-control" type="number" step="0.01" value={payForm.amount} onChange={(e) => setPayForm({ ...payForm, amount: +e.target.value })} required /></div>
                            <div className="form-group">
                                <label>Mode de paiement</label>
                                <select className="form-control" value={payForm.method} onChange={(e) => setPayForm({ ...payForm, method: e.target.value })}>
                                    <option value="Cash">Espèces</option>
                                    <option value="Card">Carte bancaire</option>
                                    <option value="BankTransfer">Virement</option>
                                </select>
                            </div>
                        </div>
                        <div className="form-group"><label>Référence</label><input className="form-control" value={payForm.referenceInfo} onChange={(e) => setPayForm({ ...payForm, referenceInfo: e.target.value })} /></div>
                        <button type="submit" className="btn btn-primary mt-1"><DollarSign size={16} /> Enregistrer</button>
                    </form>
                )}
            </div>
        </div>
    );
}
