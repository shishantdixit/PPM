'use client';

import jsPDF from 'jspdf';
import autoTable from 'jspdf-autotable';
import { Shift, FuelSale, PaymentMethod } from '@/types';
import { format } from 'date-fns';

// Extend jsPDF type to include autoTable
declare module 'jspdf' {
  interface jsPDF {
    lastAutoTable: {
      finalY: number;
    };
  }
}

interface ShiftReportData {
  shift: Shift;
  sales: FuelSale[];
  tenantName?: string;
}

export function generateShiftReportPdf(data: ShiftReportData): void {
  const { shift, sales, tenantName } = data;
  const doc = new jsPDF();

  const pageWidth = doc.internal.pageSize.getWidth();
  let yPos = 20;

  // Header
  doc.setFontSize(18);
  doc.setFont('helvetica', 'bold');
  doc.text('SHIFT REPORT', pageWidth / 2, yPos, { align: 'center' });

  yPos += 8;
  doc.setFontSize(12);
  doc.setFont('helvetica', 'normal');
  if (tenantName) {
    doc.text(tenantName, pageWidth / 2, yPos, { align: 'center' });
    yPos += 6;
  }

  // Shift Info
  yPos += 10;
  doc.setFontSize(11);
  doc.setFont('helvetica', 'bold');
  doc.text('Shift Details', 14, yPos);

  yPos += 6;
  doc.setFont('helvetica', 'normal');
  doc.setFontSize(10);

  const shiftDate = format(new Date(shift.shiftDate), 'dd MMM yyyy');
  const startTime = shift.startTime;
  const endTime = shift.endTime || 'Active';
  const statusText = ['Pending', 'Active', 'Closed'][shift.status];

  const shiftDetails = [
    ['Date:', shiftDate],
    ['Worker:', shift.workerName],
    ['Start Time:', startTime],
    ['End Time:', endTime],
    ['Status:', statusText],
  ];

  shiftDetails.forEach(([label, value]) => {
    doc.setFont('helvetica', 'bold');
    doc.text(label, 14, yPos);
    doc.setFont('helvetica', 'normal');
    doc.text(String(value), 50, yPos);
    yPos += 5;
  });

  // Nozzle Readings
  yPos += 10;
  doc.setFontSize(11);
  doc.setFont('helvetica', 'bold');
  doc.text('Nozzle Readings', 14, yPos);
  yPos += 2;

  const nozzleHeaders = [
    'Nozzle',
    'Fuel',
    'Opening',
    'Closing',
    'Qty Sold',
    'Rate',
    'Expected Amt',
  ];

  const nozzleData = shift.nozzleReadings.map((nr) => [
    nr.nozzleNumber,
    nr.fuelName,
    nr.openingReading.toFixed(2),
    nr.closingReading?.toFixed(2) || '-',
    nr.quantitySold.toFixed(2),
    `Rs ${nr.rateAtShift.toFixed(2)}`,
    `Rs ${nr.expectedAmount.toFixed(2)}`,
  ]);

  autoTable(doc, {
    startY: yPos,
    head: [nozzleHeaders],
    body: nozzleData,
    theme: 'striped',
    styles: { fontSize: 9 },
    headStyles: { fillColor: [59, 130, 246] },
    margin: { left: 14, right: 14 },
  });

  yPos = doc.lastAutoTable.finalY + 10;

  // Sales Summary
  if (sales.length > 0) {
    doc.setFontSize(11);
    doc.setFont('helvetica', 'bold');
    doc.text('Individual Sales', 14, yPos);
    yPos += 2;

    const getPaymentLabel = (method: PaymentMethod): string => {
      const labels = ['Cash', 'Credit', 'Digital', 'Mixed'];
      return labels[method];
    };

    const salesHeaders = [
      'Bill No.',
      'Time',
      'Nozzle',
      'Fuel',
      'Qty',
      'Rate',
      'Amount',
      'Payment',
      'Vehicle',
    ];

    const salesData = sales.map((s) => [
      s.saleNumber,
      format(new Date(s.saleTime), 'HH:mm'),
      s.nozzleNumber,
      s.fuelName,
      s.quantity.toFixed(2),
      `Rs ${s.rate.toFixed(2)}`,
      `Rs ${s.amount.toFixed(2)}`,
      getPaymentLabel(s.paymentMethod),
      s.vehicleNumber || '-',
    ]);

    autoTable(doc, {
      startY: yPos,
      head: [salesHeaders],
      body: salesData,
      theme: 'striped',
      styles: { fontSize: 8 },
      headStyles: { fillColor: [59, 130, 246] },
      margin: { left: 14, right: 14 },
      columnStyles: {
        0: { cellWidth: 30 },
      },
    });

    yPos = doc.lastAutoTable.finalY + 10;
  }

  // Financial Summary
  doc.setFontSize(11);
  doc.setFont('helvetica', 'bold');
  doc.text('Financial Summary', 14, yPos);
  yPos += 6;

  // Summary calculations
  const totalFromMeter = shift.totalSales;
  const cashCollected = shift.cashCollected;
  const creditSales = shift.creditSales;
  const digitalPayments = shift.digitalPayments;
  const borrowing = shift.borrowing;
  const totalCollected = cashCollected + creditSales + digitalPayments;
  const variance = shift.variance;

  // Individual sales totals
  const salesCash = sales
    .filter((s) => s.paymentMethod === 0)
    .reduce((sum, s) => sum + s.amount, 0);
  const salesCredit = sales
    .filter((s) => s.paymentMethod === 1)
    .reduce((sum, s) => sum + s.amount, 0);
  const salesDigital = sales
    .filter((s) => s.paymentMethod === 2)
    .reduce((sum, s) => sum + s.amount, 0);
  const totalFromSales = sales.reduce((sum, s) => sum + s.amount, 0);

  const summaryData = [
    ['Total Sales (Meter)', `Rs ${totalFromMeter.toFixed(2)}`],
    ['Total Sales (Individual)', `Rs ${totalFromSales.toFixed(2)}`],
    ['', ''],
    ['Cash Collected', `Rs ${cashCollected.toFixed(2)}`],
    ['Credit Sales', `Rs ${creditSales.toFixed(2)}`],
    ['Digital Payments', `Rs ${digitalPayments.toFixed(2)}`],
    ['Total Collected', `Rs ${totalCollected.toFixed(2)}`],
    ['', ''],
    ['Borrowing', `Rs ${borrowing.toFixed(2)}`],
    ['Variance', `Rs ${variance.toFixed(2)}`],
  ];

  autoTable(doc, {
    startY: yPos,
    body: summaryData,
    theme: 'plain',
    styles: { fontSize: 10 },
    margin: { left: 14, right: 100 },
    columnStyles: {
      0: { fontStyle: 'bold', cellWidth: 50 },
      1: { halign: 'right', cellWidth: 40 },
    },
  });

  yPos = doc.lastAutoTable.finalY + 10;

  // Individual sales breakdown
  if (sales.length > 0) {
    doc.setFont('helvetica', 'bold');
    doc.text('Individual Sales by Payment Method:', 110, yPos - 40);

    const paymentBreakdown = [
      ['Cash', `Rs ${salesCash.toFixed(2)}`],
      ['Credit', `Rs ${salesCredit.toFixed(2)}`],
      ['Digital', `Rs ${salesDigital.toFixed(2)}`],
      ['Total', `Rs ${totalFromSales.toFixed(2)}`],
    ];

    autoTable(doc, {
      startY: yPos - 36,
      body: paymentBreakdown,
      theme: 'plain',
      styles: { fontSize: 9 },
      margin: { left: 110, right: 14 },
      columnStyles: {
        0: { cellWidth: 30 },
        1: { halign: 'right', cellWidth: 35 },
      },
    });
  }

  // Notes
  if (shift.notes) {
    yPos = doc.lastAutoTable.finalY + 10;
    doc.setFontSize(10);
    doc.setFont('helvetica', 'bold');
    doc.text('Notes:', 14, yPos);
    doc.setFont('helvetica', 'normal');
    doc.text(shift.notes, 14, yPos + 5, { maxWidth: pageWidth - 28 });
  }

  // Footer
  const pageHeight = doc.internal.pageSize.getHeight();
  doc.setFontSize(8);
  doc.setFont('helvetica', 'normal');
  doc.text(`Generated on ${format(new Date(), 'dd MMM yyyy HH:mm')}`, 14, pageHeight - 10);
  doc.text('PPM System', pageWidth - 14, pageHeight - 10, { align: 'right' });

  // Save the PDF
  const filename = `Shift_Report_${shift.workerName.replace(/\s+/g, '_')}_${format(
    new Date(shift.shiftDate),
    'yyyy-MM-dd'
  )}.pdf`;
  doc.save(filename);
}
