import { type MeterReadingUploadResult } from '../models/meter-reading-upload-result';

const API_BASE_URL = 'https://localhost:7271';



export async function post(file: File): Promise<MeterReadingUploadResult> {
  const formData = new FormData();
  formData.append('file', file);

  const response = await fetch(`${API_BASE_URL}/meterreading/meter-reading-uploads`, {
    method: 'POST',
    body: formData,
  });

  if (!response.ok) {
    throw new Error('Upload failed.');
  }

  return await response.json();
}