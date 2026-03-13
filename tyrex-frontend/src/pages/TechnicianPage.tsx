import { useState, type FormEvent } from 'react';
import { diagnosticsApi, repairsApi, qualityApi } from '../api/endpoints';
import { Stethoscope, Play, CheckCircle, ClipboardCheck } from 'lucide-react';

export default function TechnicianPage() {
    const [activeTab, setActiveTab] = useState<'diag' | 'start' | 'complete' | 'quality'>('diag');
    const [message, setMessage] = useState('');

    // Diagnosis
    const [diagForm, setDiagForm] = useState({ repairOrderId: '', technicianId: '', notes: '' });
    // Start repair
    const [startForm, setStartForm] = useState({ repairOrderId: '', technicianId: '' });
    // Complete repair
    const [completeForm, setCompleteForm] = useState({ repairOrderId: '', technicianId: '' });
    // Quality
    const [qualityForm, setQualityForm] = useState({ repairOrderId: '', inspectorId: '', itemName: '', finalNotes: '' });

    const handleDiag = async (e: FormEvent) => {
        e.preventDefault();
        try {
            await diagnosticsApi.submit(diagForm.repairOrderId, { technicianId: diagForm.technicianId, notes: diagForm.notes, mediaUrls: [] });
            setMessage('✓ Diagnostic soumis');
        } catch (err: any) { setMessage(`✗ ${err.response?.data?.message || err.message}`); }
    };

    const handleStart = async (e: FormEvent) => {
        e.preventDefault();
        try {
            await repairsApi.start(startForm.repairOrderId, { technicianId: startForm.technicianId });
            setMessage('✓ Réparation démarrée');
        } catch (err: any) { setMessage(`✗ ${err.response?.data?.message || err.message}`); }
    };

    const handleComplete = async (e: FormEvent) => {
        e.preventDefault();
        try {
            await repairsApi.complete(completeForm.repairOrderId, { technicianId: completeForm.technicianId });
            setMessage('✓ Réparation terminée');
        } catch (err: any) { setMessage(`✗ ${err.response?.data?.message || err.message}`); }
    };

    const handleQuality = async (e: FormEvent) => {
        e.preventDefault();
        try {
            await qualityApi.submitChecklist(qualityForm.repairOrderId, {
                inspectorId: qualityForm.inspectorId,
                items: [{ name: qualityForm.itemName || 'General Check', description: 'Visual inspection', status: 1, notes: '' }],
                finalNotes: qualityForm.finalNotes,
            });
            setMessage('✓ Contrôle qualité soumis');
        } catch (err: any) { setMessage(`✗ ${err.response?.data?.message || err.message}`); }
    };

    const tabs = [
        { key: 'diag' as const, label: 'Diagnostic', icon: Stethoscope },
        { key: 'start' as const, label: 'Démarrer', icon: Play },
        { key: 'complete' as const, label: 'Terminer', icon: CheckCircle },
        { key: 'quality' as const, label: 'Qualité', icon: ClipboardCheck },
    ];

    return (
        <div>
            <div className="page-header">
                <h2>Atelier Technicien</h2>
                <p>Diagnostic, Réparation et Contrôle Qualité</p>
            </div>

            {message && <div className="card mt-1" style={{ marginBottom: '1rem', padding: '0.8rem 1rem', fontSize: '0.88rem' }}>{message}</div>}

            <div className="flex gap-1" style={{ marginBottom: '1.5rem' }}>
                {tabs.map((t) => (
                    <button key={t.key} className={`btn ${activeTab === t.key ? 'btn-primary' : 'btn-secondary'}`} onClick={() => setActiveTab(t.key)}>
                        <t.icon size={16} /> {t.label}
                    </button>
                ))}
            </div>

            <div className="card">
                {activeTab === 'diag' && (
                    <form onSubmit={handleDiag}>
                        <h3 style={{ marginBottom: '1rem' }}>Soumettre un Diagnostic</h3>
                        <div className="form-row">
                            <div className="form-group"><label>ID Ordre de réparation</label><input className="form-control" value={diagForm.repairOrderId} onChange={(e) => setDiagForm({ ...diagForm, repairOrderId: e.target.value })} required /></div>
                            <div className="form-group"><label>ID Technicien</label><input className="form-control" value={diagForm.technicianId} onChange={(e) => setDiagForm({ ...diagForm, technicianId: e.target.value })} required /></div>
                        </div>
                        <div className="form-group"><label>Notes</label><textarea className="form-control" rows={3} value={diagForm.notes} onChange={(e) => setDiagForm({ ...diagForm, notes: e.target.value })} /></div>
                        <button type="submit" className="btn btn-primary mt-1"><Stethoscope size={16} /> Soumettre</button>
                    </form>
                )}
                {activeTab === 'start' && (
                    <form onSubmit={handleStart}>
                        <h3 style={{ marginBottom: '1rem' }}>Démarrer la réparation</h3>
                        <div className="form-row">
                            <div className="form-group"><label>ID Ordre de réparation</label><input className="form-control" value={startForm.repairOrderId} onChange={(e) => setStartForm({ ...startForm, repairOrderId: e.target.value })} required /></div>
                            <div className="form-group"><label>ID Technicien</label><input className="form-control" value={startForm.technicianId} onChange={(e) => setStartForm({ ...startForm, technicianId: e.target.value })} required /></div>
                        </div>
                        <button type="submit" className="btn btn-primary mt-1"><Play size={16} /> Démarrer</button>
                    </form>
                )}
                {activeTab === 'complete' && (
                    <form onSubmit={handleComplete}>
                        <h3 style={{ marginBottom: '1rem' }}>Terminer la réparation</h3>
                        <div className="form-row">
                            <div className="form-group"><label>ID Ordre de réparation</label><input className="form-control" value={completeForm.repairOrderId} onChange={(e) => setCompleteForm({ ...completeForm, repairOrderId: e.target.value })} required /></div>
                            <div className="form-group"><label>ID Technicien</label><input className="form-control" value={completeForm.technicianId} onChange={(e) => setCompleteForm({ ...completeForm, technicianId: e.target.value })} required /></div>
                        </div>
                        <button type="submit" className="btn btn-primary mt-1"><CheckCircle size={16} /> Terminer</button>
                    </form>
                )}
                {activeTab === 'quality' && (
                    <form onSubmit={handleQuality}>
                        <h3 style={{ marginBottom: '1rem' }}>Contrôle Qualité</h3>
                        <div className="form-row">
                            <div className="form-group"><label>ID Ordre de réparation</label><input className="form-control" value={qualityForm.repairOrderId} onChange={(e) => setQualityForm({ ...qualityForm, repairOrderId: e.target.value })} required /></div>
                            <div className="form-group"><label>ID Inspecteur</label><input className="form-control" value={qualityForm.inspectorId} onChange={(e) => setQualityForm({ ...qualityForm, inspectorId: e.target.value })} required /></div>
                        </div>
                        <div className="form-group"><label>Notes finales</label><textarea className="form-control" rows={2} value={qualityForm.finalNotes} onChange={(e) => setQualityForm({ ...qualityForm, finalNotes: e.target.value })} /></div>
                        <button type="submit" className="btn btn-primary mt-1"><ClipboardCheck size={16} /> Soumettre le contrôle</button>
                    </form>
                )}
            </div>
        </div>
    );
}
