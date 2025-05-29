import React, { useRef, useState } from 'react';
import { post } from '../api/meter-reading-api';
import type { MeterReadingUploadResult } from '../models/meter-reading-upload-result';

const MeterReadingUpload: React.FC = () => {
    const [selectedFile, setSelectedFile] = useState<File | null>(null);
    const [result, setResult] = useState<MeterReadingUploadResult | null>(null);
    const [error, setError] = useState<string | null>(null);
    const [isSubmitting, setIsSubmitting] = useState(false);
    const fileInputRef = useRef<HTMLInputElement>(null);
    const [selectedFileName, setSelectedFileName] = useState<string | null>(null);

    const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const file = e.target.files?.[0];
        if (!file) return;
        else setSelectedFileName(file.name);

        if (!file.name.endsWith('.csv')) {
            setSelectedFileName(null);
            setError('Please upload a valid CSV file.');
            return;
        }

        setSelectedFileName(file.name);
        setSelectedFile(file);
        setError(null);
        setResult(null);
    };

    const handleSubmit = async () => {
        if (!selectedFile) {
            setError('Please select a CSV file first.');
            return;
        }

        try {
            setIsSubmitting(true);
            const response = await post(selectedFile);
            setResult(response);
            setError(null);
        } catch {
            setError('Upload failed. Please try again.');
            setResult(null);
        } finally {
            setIsSubmitting(false);
        }
    };

    const handleButtonClick = () => {
        fileInputRef.current?.click();
    };


    return (
        <div className="container py-5" style={{ fontFamily: 'Merriweather, serif' }}>
            <div className="row justify-content-center">
                <div className="col-md-8 col-lg-6">
                    <div className="card border-0 shadow-lg">
                        <div className="card-body p-5">
                            <h2 className="text-center mb-4 fw-bold">Upload Meter Readings</h2>
                            <div className="mb-3">
                            </div>
                            <div className="mb-3">



                                {/* Hidden input */}
                                <input
                                    type="file"
                                    accept=".csv"
                                    ref={fileInputRef}
                                    onChange={handleFileChange}
                                    style={{ display: 'none' }}
                                />

                                {/* Button */}
                                <button
                                    type="button"
                                    className="btn btn-lg btn-dark w-100"
                                    onClick={handleButtonClick}
                                >
                                    Upload CSV
                                </button>

                                {/* File name display */}
                                {selectedFileName && (
                                    <div className="mt-2 text-muted text-center">
                                        <strong>{selectedFileName}</strong>
                                    </div>
                                )}
                            </div>

                            <div className="mb-4">
                                <button
                                    className="btn btn-primary w-100"
                                    onClick={handleSubmit}
                                    disabled={!selectedFile || isSubmitting}
                                >
                                    {isSubmitting ? 'Uploading...' : 'Submit'}
                                </button>
                            </div>

                            {error && (
                                <div className="alert alert-danger mt-3 mb-0" role="alert">
                                    {error}
                                </div>
                            )}

                            {result && (
                                <div className="alert alert-success mt-3 mb-0">
                                    <h2 className="text-center mb-4">Upload Summary</h2>
                                    <div className="d-flex justify-content-between">
                                        <span>Total Records</span>
                                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                        <span className="fw-semibold">{result.TotalRecords}</span>
                                    </div>
                                    <div className="d-flex justify-content-between">
                                        <span className="text-success">Successful</span>
                                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                        <span className="fw-semibold text-success">{result.SuccessfulRecords}</span>
                                    </div>
                                    <div className="d-flex justify-content-between">
                                        <span className="text-danger">Failed</span>
                                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                        <span className="fw-semibold text-danger">{result.FailedRecords}</span>
                                    </div>
                                </div>
                            )}
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default MeterReadingUpload;
